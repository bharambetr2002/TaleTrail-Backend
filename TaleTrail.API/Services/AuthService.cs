using Supabase.Gotrue;
using TaleTrail.API.DAO;
using TaleTrail.API.DTOs.Auth;
using TaleTrail.API.Exceptions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using UserModel = TaleTrail.API.Models.User;

namespace TaleTrail.API.Services
{
    public class AuthService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly UserDao _userDao;
        private readonly ILogger<AuthService> _logger;

        public AuthService(SupabaseService supabaseService, UserDao userDao, ILogger<AuthService> logger)
        {
            _supabaseClient = supabaseService.Client;
            _userDao = userDao;
            _logger = logger;
        }

        public async Task<UserResponseDTO> SignupAsync(SignupDTO request)
        {
            _logger.LogInformation("🔐 Starting signup process for email: {Email}", request.Email);

            // Pre-check if username or email already exists in our database
            if (await _userDao.UsernameExistsAsync(request.Username))
            {
                throw new AppException($"Username '{request.Username}' is already taken. Please choose another.");
            }

            if (await _userDao.EmailExistsAsync(request.Email))
            {
                throw new AppException($"An account with email '{request.Email}' already exists. Please login instead.");
            }

            Session? session;
            try
            {
                _logger.LogDebug("📝 Creating Supabase Auth user for: {Email}", request.Email);
                session = await _supabaseClient.Auth.SignUp(request.Email, request.Password, new SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        { "full_name", request.FullName },
                        { "username", request.Username }
                    }
                });

                if (session?.User?.Id == null)
                {
                    _logger.LogError("❌ Supabase signup failed - no user ID returned for: {Email}", request.Email);
                    throw new AppException("Signup failed. User was not created by Supabase Auth.");
                }

                _logger.LogInformation("✅ Supabase Auth user created successfully: {UserId}", session.User.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Supabase Auth signup failed for email {Email}", request.Email);

                if (ex.Message.Contains("User already registered") || ex.Message.Contains("user_already_exists"))
                    throw new AppException("An account with this email already exists. Please login instead.");

                if (ex.Message.Contains("Invalid email"))
                    throw new AppException("Please provide a valid email address.");

                if (ex.Message.Contains("Password"))
                    throw new AppException("Password must be at least 6 characters long.");

                throw new AppException($"An authentication error occurred: {ex.Message}");
            }

            // Create user profile in our database
            var userProfile = new UserModel
            {
                Id = Guid.Parse(session.User.Id),
                Email = request.Email.ToLowerInvariant(),
                FullName = request.FullName.Trim(),
                Username = request.Username.ToLowerInvariant().Trim(),
                CreatedAt = DateTime.UtcNow,
                Role = "user" // Default role
            };

            try
            {
                _logger.LogDebug("💾 Creating user profile in database for: {UserId}", userProfile.Id);
                var createdProfile = await _userDao.AddAsync(userProfile);
                if (createdProfile == null)
                {
                    _logger.LogError("❌ Failed to create user profile in database for auth user {UserId}", userProfile.Id);
                    throw new AppException("Your account was created, but we failed to save your profile. Please contact support.");
                }

                _logger.LogInformation("✅ User profile created successfully: {UserId} ({Username})", createdProfile.Id, createdProfile.Username);

                return new UserResponseDTO
                {
                    Email = createdProfile.Email,
                    FullName = createdProfile.FullName,
                    UserId = createdProfile.Id,
                    AccessToken = session.AccessToken ?? string.Empty,
                    RefreshToken = session.RefreshToken ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to create user profile for {UserId}", userProfile.Id);

                // Try to clean up the Supabase Auth user if profile creation failed
                try
                {
                    // Note: Supabase doesn't easily allow deleting users from client SDK
                    // You'd need admin privileges for this
                    _logger.LogWarning("⚠️ Consider manual cleanup of Supabase Auth user: {UserId}", userProfile.Id);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx, "❌ Failed to cleanup Supabase Auth user after profile creation failure");
                }

                if (ex.Message.Contains("duplicate key") || ex.Message.Contains("23505"))
                {
                    if (ex.Message.Contains("username"))
                        throw new AppException($"Username '{request.Username}' is already taken.");
                    if (ex.Message.Contains("email"))
                        throw new AppException($"An account with email '{request.Email}' already exists.");
                    throw new AppException("A user with this username or email already exists.");
                }

                throw new AppException($"An error occurred while creating your profile: {ex.Message}");
            }
        }

        public async Task<UserResponseDTO> LoginAsync(LoginDTO request)
        {
            _logger.LogInformation("🔐 Starting login process for email: {Email}", request.Email);

            try
            {
                _logger.LogDebug("🔍 Authenticating with Supabase Auth for: {Email}", request.Email);
                var session = await _supabaseClient.Auth.SignIn(request.Email, request.Password);

                if (session?.User?.Id == null)
                {
                    _logger.LogWarning("❌ Supabase Auth login failed for: {Email}", request.Email);
                    throw new AppException("Invalid email or password.");
                }

                var userId = Guid.Parse(session.User.Id);
                _logger.LogDebug("✅ Supabase Auth successful for user: {UserId}", userId);

                // Get user profile from our database
                var user = await _userDao.GetByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("⚠️ User profile not found for authenticated user {UserId}, attempting recovery", userId);

                    // Try to recover by creating the missing profile
                    var recoveredUser = await RecoverMissingUserProfile(session.User, request.Email);
                    if (recoveredUser != null)
                    {
                        user = recoveredUser;
                        _logger.LogInformation("✅ Successfully recovered missing user profile for {UserId}", userId);
                    }
                    else
                    {
                        _logger.LogError("❌ Failed to recover user profile for {UserId}", userId);
                        throw new AppException("Account setup incomplete. Please contact support or try signing up again.");
                    }
                }

                _logger.LogInformation("✅ Login successful for user: {UserId} ({Username})", user.Id, user.Username);

                return new UserResponseDTO
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    UserId = user.Id,
                    AccessToken = session.AccessToken ?? string.Empty,
                    RefreshToken = session.RefreshToken ?? string.Empty
                };
            }
            catch (AppException)
            {
                throw; // Re-throw our custom exceptions
            }
            catch (Supabase.Gotrue.Exceptions.GotrueException ex)
            {
                _logger.LogWarning("❌ Supabase Auth error for {Email}: {Error}", request.Email, ex.Message);

                if (ex.Message.Contains("Invalid login credentials") || ex.Message.Contains("invalid_credentials"))
                    throw new AppException("Invalid email or password. Please check your credentials and try again.");

                if (ex.Message.Contains("Email not confirmed"))
                    throw new AppException("Please check your email and click the confirmation link before logging in.");

                if (ex.Message.Contains("Too many requests"))
                    throw new AppException("Too many login attempts. Please wait a moment and try again.");

                throw new AppException($"Authentication failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error during login for email {Email}", request.Email);
                throw new AppException("Login failed due to an unexpected error. Please try again.");
            }
        }

        /// <summary>
        /// Attempts to recover a missing user profile by creating it from Supabase Auth data
        /// </summary>
        private async Task<UserModel?> RecoverMissingUserProfile(User supabaseUser, string email)
        {
            try
            {
                var userId = Guid.Parse(supabaseUser.Id);

                // Generate a unique username for recovery
                var username = await GenerateUniqueUsernameAsync(supabaseUser, email);
                var fullName = GetFullNameFromUser(supabaseUser) ?? "User";

                var recoveryProfile = new UserModel
                {
                    Id = userId,
                    Email = email.ToLowerInvariant(),
                    FullName = fullName,
                    Username = username,
                    CreatedAt = DateTime.UtcNow,
                    Role = "user"
                };

                var createdUser = await _userDao.AddAsync(recoveryProfile);
                if (createdUser != null)
                {
                    _logger.LogInformation("✅ Successfully created recovery profile for user: {UserId}", userId);
                    return createdUser;
                }
                else
                {
                    _logger.LogError("❌ Failed to create recovery profile for user: {UserId}", userId);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during user profile recovery for user: {UserId}", supabaseUser.Id);

                // If recovery fails due to conflicts, try to find existing user by email
                if (ex.Message.Contains("duplicate key") || ex.Message.Contains("23505"))
                {
                    try
                    {
                        var existingUser = await _userDao.GetByEmailAsync(email);
                        if (existingUser != null)
                        {
                            _logger.LogInformation("✅ Found existing user by email during recovery: {UserId}", existingUser.Id);
                            return existingUser;
                        }
                    }
                    catch (Exception findEx)
                    {
                        _logger.LogError(findEx, "❌ Failed to find existing user by email during recovery");
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Generates a unique username from Supabase user data or email
        /// </summary>
        private async Task<string> GenerateUniqueUsernameAsync(User user, string? fallbackEmail = null)
        {
            // Try to use username from Supabase metadata first
            if (user.UserMetadata != null && user.UserMetadata.ContainsKey("username"))
            {
                var metadataUsername = user.UserMetadata["username"]?.ToString();
                if (!string.IsNullOrEmpty(metadataUsername))
                {
                    var cleanUsername = CleanUsername(metadataUsername);
                    if (!(await _userDao.UsernameExistsAsync(cleanUsername)))
                        return cleanUsername;
                }
            }

            // Generate from email
            string baseUsername;
            var email = !string.IsNullOrEmpty(user.Email) ? user.Email : fallbackEmail;
            if (!string.IsNullOrEmpty(email))
            {
                baseUsername = CleanUsername(email.Split('@')[0]);
            }
            else
            {
                baseUsername = "user";
            }

            // Find unique variation
            var counter = 1;
            var candidateUsername = baseUsername;

            while (await _userDao.UsernameExistsAsync(candidateUsername))
            {
                candidateUsername = $"{baseUsername}_{counter}";
                counter++;

                // Prevent infinite loop
                if (counter > 1000)
                {
                    candidateUsername = $"user_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Guid.NewGuid().ToString("N")[..8]}";
                    break;
                }
            }

            return candidateUsername;
        }

        /// <summary>
        /// Cleans and validates username format
        /// </summary>
        private string CleanUsername(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "user";

            // Remove invalid characters and convert to lowercase
            var cleaned = System.Text.RegularExpressions.Regex.Replace(input.ToLowerInvariant(), @"[^a-z0-9_]", "");

            // Ensure it starts with a letter or underscore
            if (!char.IsLetter(cleaned[0]) && cleaned[0] != '_')
                cleaned = "u" + cleaned;

            // Ensure minimum length
            if (cleaned.Length < 3)
                cleaned = cleaned.PadRight(3, '1');

            // Ensure maximum length
            if (cleaned.Length > 50)
                cleaned = cleaned.Substring(0, 50);

            return cleaned;
        }

        /// <summary>
        /// Extracts full name from Supabase user metadata
        /// </summary>
        private string? GetFullNameFromUser(User user)
        {
            if (user.UserMetadata != null && user.UserMetadata.ContainsKey("full_name"))
            {
                var fullName = user.UserMetadata["full_name"]?.ToString();
                if (!string.IsNullOrEmpty(fullName))
                    return fullName.Trim();
            }

            // Fallback to email username
            if (!string.IsNullOrEmpty(user.Email))
                return user.Email.Split('@')[0];

            return null;
        }

        /// <summary>
        /// Validates user credentials without creating a session (for API validation)
        /// </summary>
        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            try
            {
                var session = await _supabaseClient.Auth.SignIn(email, password);
                if (session?.User?.Id != null)
                {
                    // Sign out immediately to avoid creating a persistent session
                    await _supabaseClient.Auth.SignOut();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
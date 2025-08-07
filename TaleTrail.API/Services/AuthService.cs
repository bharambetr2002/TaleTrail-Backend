// 🔧 FIXED AuthService.cs - Forces UUID synchronization
using Supabase.Gotrue;
using TaleTrail.API.DAO;
using TaleTrail.API.DTOs.Auth;
using TaleTrail.API.Exceptions;
using Microsoft.Extensions.Logging;
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

            // 1. Pre-check our database first
            if (await _userDao.EmailExistsAsync(request.Email))
            {
                throw new AppException($"An account with email '{request.Email}' already exists. Please login instead.");
            }

            if (await _userDao.UsernameExistsAsync(request.Username))
            {
                throw new AppException($"Username '{request.Username}' is already taken. Please choose another.");
            }

            // 2. Create Supabase Auth user
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
                    throw new AppException("Signup failed. User was not created by Supabase Auth.");
                }

                _logger.LogInformation("✅ Supabase Auth user created with ID: {SupabaseUserId}", session.User.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Supabase Auth signup failed for email {Email}", request.Email);
                throw new AppException($"Authentication error: {ex.Message}");
            }

            // 3. ✅ CRITICAL FIX: Use Supabase's UUID directly
            var user = await CreateUserInDatabase(session.User.Id, request.Email, request.FullName, request.Username);

            return new UserResponseDTO
            {
                Email = user.Email,
                FullName = user.FullName,
                UserId = user.Id,
                AccessToken = session.AccessToken ?? string.Empty,
                RefreshToken = session.RefreshToken ?? string.Empty
            };
        }

        public async Task<UserResponseDTO> LoginAsync(LoginDTO request)
        {
            _logger.LogInformation("🔐 Starting login process for email: {Email}", request.Email);

            Session? session;
            try
            {
                session = await _supabaseClient.Auth.SignIn(request.Email, request.Password);

                if (session?.User?.Id == null)
                {
                    throw new AppException("Invalid email or password.");
                }

                _logger.LogDebug("✅ Supabase Auth successful for user: {UserId}", session.User.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Supabase Auth login failed for email {Email}", request.Email);
                throw new AppException("Authentication failed. Please check your credentials.");
            }

            // ✅ CRITICAL FIX: Always ensure user exists with Supabase's exact UUID
            var user = await EnsureUserExistsFromSupabaseId(session.User.Id, session.User.Email ?? request.Email);

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

        /// <summary>
        /// ✅ FIXED: Creates user using Supabase's exact UUID
        /// </summary>
        private async Task<UserModel> CreateUserInDatabase(string supabaseUserId, string email, string fullName, string username)
        {
            try
            {
                // ✅ CRITICAL: Parse Supabase's UUID and use it directly
                var userId = Guid.Parse(supabaseUserId);

                _logger.LogInformation("🔗 Creating user in database with Supabase UUID: {UserId}", userId);

                var newUser = new UserModel
                {
                    Id = userId, // ✅ Use Supabase's exact UUID
                    Email = email.ToLowerInvariant(),
                    FullName = fullName.Trim(),
                    Username = username.ToLowerInvariant().Trim(),
                    CreatedAt = DateTime.UtcNow,
                    Role = "user"
                };

                var createdUser = await _userDao.AddAsync(newUser);
                if (createdUser == null)
                {
                    throw new AppException("Failed to create user profile in database");
                }

                _logger.LogInformation("✅ User profile created with synchronized UUID: {UserId} ({Username})", createdUser.Id, createdUser.Username);
                return createdUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to create user profile for Supabase user: {UserId}", supabaseUserId);

                // Handle race conditions
                if (ex.Message.Contains("duplicate key") || ex.Message.Contains("23505"))
                {
                    _logger.LogWarning("🔄 Duplicate user detected, attempting to retrieve existing user");
                    var existingUser = await _userDao.GetByIdAsync(Guid.Parse(supabaseUserId));
                    if (existingUser != null)
                    {
                        _logger.LogInformation("✅ Found existing user with synchronized UUID: {UserId}", existingUser.Id);
                        return existingUser;
                    }
                }

                throw new AppException($"Failed to create user profile: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ FIXED: Ensures user exists with Supabase's exact UUID
        /// </summary>
        private async Task<UserModel> EnsureUserExistsFromSupabaseId(string supabaseUserId, string email)
        {
            var userId = Guid.Parse(supabaseUserId);

            _logger.LogDebug("🔍 Looking for user with Supabase UUID: {UserId}", userId);

            // Try to get existing user with Supabase's UUID
            var existingUser = await _userDao.GetByIdAsync(userId);
            if (existingUser != null)
            {
                _logger.LogDebug("✅ User found with synchronized UUID: {UserId}", userId);
                return existingUser;
            }

            // User doesn't exist with Supabase UUID, create it
            _logger.LogWarning("🔧 User {UserId} missing from database, creating with Supabase UUID", userId);

            try
            {
                var username = await GenerateUsernameFromEmail(email);
                var fullName = ExtractFullNameFromEmail(email);

                var newUser = new UserModel
                {
                    Id = userId, // ✅ CRITICAL: Use Supabase's exact UUID
                    Email = email.ToLowerInvariant(),
                    FullName = fullName,
                    Username = username,
                    CreatedAt = DateTime.UtcNow,
                    Role = "user"
                };

                var createdUser = await _userDao.AddAsync(newUser);
                if (createdUser == null)
                {
                    throw new AppException("Failed to create missing user profile");
                }

                _logger.LogInformation("✅ Auto-created user with synchronized UUID: {UserId} ({Username})", createdUser.Id, createdUser.Username);
                return createdUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to auto-create user {UserId}", userId);
                throw new AppException($"Failed to synchronize user profile: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates unique username from email
        /// </summary>
        private async Task<string> GenerateUsernameFromEmail(string email)
        {
            var baseUsername = CleanUsername(email.Split('@')[0]);
            var counter = 1;
            var candidateUsername = baseUsername;

            while (await _userDao.UsernameExistsAsync(candidateUsername))
            {
                candidateUsername = $"{baseUsername}_{counter}";
                counter++;

                if (counter > 1000)
                {
                    candidateUsername = $"user_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                    break;
                }
            }

            return candidateUsername;
        }

        /// <summary>
        /// Cleans username format
        /// </summary>
        private string CleanUsername(string input)
        {
            if (string.IsNullOrEmpty(input)) return "user";

            var cleaned = System.Text.RegularExpressions.Regex.Replace(input.ToLowerInvariant(), @"[^a-z0-9_]", "");

            if (cleaned.Length == 0 || (!char.IsLetter(cleaned[0]) && cleaned[0] != '_'))
                cleaned = "u" + cleaned;

            if (cleaned.Length < 3)
                cleaned = cleaned.PadRight(3, '1');

            if (cleaned.Length > 50)
                cleaned = cleaned.Substring(0, 50);

            return cleaned;
        }

        /// <summary>
        /// Extracts full name from email
        /// </summary>
        private string ExtractFullNameFromEmail(string email)
        {
            try
            {
                var localPart = email.Split('@')[0];
                var name = localPart.Replace(".", " ").Replace("_", " ");
                return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
            }
            catch
            {
                return "User";
            }
        }

        // Keep existing methods for backward compatibility
        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            try
            {
                var session = await _supabaseClient.Auth.SignIn(email, password);
                if (session?.User?.Id != null)
                {
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
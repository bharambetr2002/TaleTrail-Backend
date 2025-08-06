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
            Session? session;
            try
            {
                session = await _supabaseClient.Auth.SignUp(request.Email, request.Password, new SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        { "full_name", request.FullName },
                        { "username", request.Username }
                    }
                });

                if (session?.User?.Id == null)
                    throw new AppException("Signup failed. User was not created by Supabase Auth.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Supabase Auth signup failed for email {Email}", request.Email);

                if (ex.Message.Contains("User already registered") || ex.Message.Contains("user_already_exists"))
                    throw new AppException("An account with this email already exists.");

                throw new AppException($"An authentication error occurred: {ex.Message}");
            }

            var uniqueUsername = await GenerateUniqueUsernameAsync(session.User); // ✅ Added this

            var userProfile = new UserModel
            {
                Id = Guid.Parse(session.User.Id),
                Email = request.Email,
                FullName = request.FullName,
                Username = uniqueUsername, // ✅ Changed here
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var createdProfile = await _userDao.AddAsync(userProfile);
                if (createdProfile == null)
                {
                    _logger.LogError("Failed to create user profile in database for auth user {UserId}", userProfile.Id);
                    throw new AppException("Your account was created, but we failed to save your profile. Please contact support.");
                }

                _logger.LogInformation("User profile created successfully for {UserId}", createdProfile.Id);

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
                _logger.LogError(ex, "Failed to create user profile for {UserId}", userProfile.Id);
                if (ex.Message.Contains("duplicate key") || ex.Message.Contains("23505"))
                    throw new AppException("A user with this username or email already exists.");

                throw new AppException($"An error occurred while creating your profile: {ex.Message}");
            }
        }

        public async Task<UserResponseDTO> LoginAsync(LoginDTO request)
        {
            try
            {
                var session = await _supabaseClient.Auth.SignIn(request.Email, request.Password);

                if (session?.User?.Id == null)
                    throw new AppException("Invalid email or password.");

                var userId = Guid.Parse(session.User.Id);
                var user = await _userDao.GetByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("User profile not found for authenticated user {UserId}, attempting to create profile", session.User.Id);

                    var username = await GenerateUniqueUsernameAsync(session.User);

                    user = new UserModel
                    {
                        Id = userId,
                        Email = session.User.Email ?? request.Email,
                        FullName = GetFullNameFromUser(session.User) ?? "Unknown User",
                        Username = username,
                        CreatedAt = DateTime.UtcNow
                    };

                    try
                    {
                        var createdUser = await _userDao.AddAsync(user);
                        if (createdUser != null)
                        {
                            user = createdUser;
                            _logger.LogInformation("Successfully created missing user profile for {UserId}", userId);
                        }
                        else
                        {
                            throw new AppException("Failed to create user profile during login recovery.");
                        }
                    }
                    catch (Exception profileEx)
                    {
                        _logger.LogError(profileEx, "Failed to create user profile during login recovery for {UserId}", userId);

                        if (profileEx.Message.Contains("duplicate key") || profileEx.Message.Contains("23505"))
                        {
                            var users = await _userDao.GetAllUsersAsync();
                            user = users.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

                            if (user == null)
                                throw new AppException("Account setup incomplete. Please contact support.");
                        }
                        else
                        {
                            throw new AppException("Login failed due to profile setup issues. Please contact support.");
                        }
                    }
                }

                return new UserResponseDTO
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    UserId = user.Id,
                    AccessToken = session.AccessToken ?? string.Empty,
                    RefreshToken = session.RefreshToken ?? string.Empty
                };
            }
            catch (AppException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for email {Email}", request.Email);
                throw new AppException("Login failed. Please check your credentials and try again.");
            }
        }

        private async Task<string> GenerateUniqueUsernameAsync(User user)
        {
            if (user.UserMetadata != null && user.UserMetadata.ContainsKey("username"))
            {
                var metadataUsername = user.UserMetadata["username"]?.ToString();
                if (!string.IsNullOrEmpty(metadataUsername))
                {
                    var existingUser = await _userDao.GetByUsernameAsync(metadataUsername);
                    if (existingUser == null)
                        return metadataUsername;
                }
            }

            string baseUsername = !string.IsNullOrEmpty(user.Email)
                ? user.Email.Split('@')[0]
                : "user";

            var counter = 1;
            var candidateUsername = baseUsername;

            while (await _userDao.UsernameExistsAsync(candidateUsername))
            {
                candidateUsername = $"{baseUsername}_{counter}";
                counter++;

                if (counter > 1000)
                {
                    candidateUsername = $"user_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Guid.NewGuid().ToString("N")[..8]}";
                    break;
                }
            }

            return candidateUsername;
        }

        private string? GetFullNameFromUser(User user)
        {
            if (user.UserMetadata != null && user.UserMetadata.ContainsKey("full_name"))
                return user.UserMetadata["full_name"]?.ToString();

            if (!string.IsNullOrEmpty(user.Email))
                return user.Email.Split('@')[0];

            return null;
        }
    }
}

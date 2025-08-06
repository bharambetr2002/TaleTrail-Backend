using Supabase.Gotrue;
using TaleTrail.API.DAO;
using TaleTrail.API.DTOs.Auth;
using TaleTrail.API.Exceptions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using UserModel = TaleTrail.API.Models.User;
using TaleTrail.API.DTOs.Auth.Signup;

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
            // Step 1: Create user in Supabase Auth
            Session? session;
            try
            {
                session = await _supabaseClient.Auth.SignUp(request.Email, request.Password, new SignUpOptions
                {
                    Data = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "full_name", request.FullName },
                        { "username", request.Username }
                    }
                });

                if (session?.User == null)
                {
                    throw new AppException("Signup failed. User was not created by Supabase Auth.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Supabase Auth signup failed for email {Email}", request.Email);
                // Check for a common Supabase error message
                if (ex.Message.Contains("User already registered"))
                {
                    throw new AppException("An account with this email already exists.");
                }
                throw new AppException($"An authentication error occurred: {ex.Message}");
            }

            // Step 2: Create user profile in our public.users table
            var userProfile = new UserModel
            {
                Id = Guid.Parse(session.User.Id), // Use the same ID from Supabase Auth
                Email = request.Email,
                FullName = request.FullName,
                Username = request.Username,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var createdProfile = await _userDao.AddAsync(userProfile);
                if (createdProfile == null)
                {
                    // This is a critical failure state. We should ideally delete the auth user here.
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
                // Handle potential race condition or unique constraint violation
                if (ex.Message.Contains("duplicate key"))
                {
                    throw new AppException("A user with this username or email already exists.");
                }
                throw new AppException($"An error occurred while creating your profile: {ex.Message}");
            }
        }

        public async Task<UserResponseDTO> LoginAsync(LoginDTO request)
        {
            try
            {
                var session = await _supabaseClient.Auth.SignIn(request.Email, request.Password);

                if (session?.User == null)
                    throw new AppException("Invalid email or password.");

                var user = await _userDao.GetByIdAsync(Guid.Parse(session.User.Id));
                if (user == null)
                {
                    // This is a recovery mechanism in case the user exists in auth but not in our public table
                    _logger.LogWarning("User profile not found for authenticated user {UserId}, creating new profile", session.User.Id);
                    user = new UserModel
                    {
                        Id = Guid.Parse(session.User.Id),
                        Email = session.User.Email!,
                        FullName = session.User.UserMetadata["full_name"].ToString()!,
                        Username = session.User.UserMetadata["username"].ToString()!,
                    };
                    await _userDao.AddAsync(user);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for email {Email}", request.Email);
                throw new AppException("Login failed. Please check your credentials and try again.");
            }
        }
    }
}

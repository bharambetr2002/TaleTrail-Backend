using Supabase.Gotrue;
using TaleTrail.API.DTOs.Auth;
using TaleTrail.API.DTOs.Auth.Signup;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;
using UserModel = TaleTrail.API.Models.User;
using AuthUser = Supabase.Gotrue.User;

namespace TaleTrail.API.Services
{
    public class AuthService
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<AuthService> _logger;

        public AuthService(SupabaseService supabase, ILogger<AuthService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<UserResponseDTO> SignupAsync(SignupDTO request)
        {
            ValidationHelper.ValidateModel(request);

            if (!ValidationHelper.IsValidEmail(request.Email))
                throw new ValidationException("Invalid email format.");

            _logger.LogInformation("Starting signup process for email: {Email}", request.Email);

            // Check if user already exists in our database
            var existingUser = await GetUserProfileByEmail(request.Email);
            if (existingUser != null)
            {
                throw new AppException("An account with this email already exists. Please try logging in instead.");
            }

            Session? authResult = null;

            try
            {
                // Step 1: Create user in Supabase Auth
                authResult = await _supabase.Client.Auth.SignUp(request.Email, request.Password);

                if (authResult?.User == null)
                    throw new AppException("Signup failed. User was not created by Supabase Auth.");

                var userId = Guid.Parse(authResult.User.Id!);
                _logger.LogInformation("Supabase Auth user created with ID: {UserId}", userId);

                // Step 2: Check if user profile already exists (edge case)
                var existingProfile = await GetUserProfileById(userId);
                if (existingProfile != null)
                {
                    _logger.LogInformation("User profile already exists for {UserId}", userId);
                    return new UserResponseDTO
                    {
                        Email = existingProfile.Email,
                        FullName = existingProfile.FullName,
                        UserId = existingProfile.Id,
                        AccessToken = authResult.AccessToken ?? string.Empty,
                        RefreshToken = authResult.RefreshToken ?? string.Empty
                    };
                }

                // Step 3: Create user profile in our database
                var userProfile = new UserModel
                {
                    Id = userId, // Use the same ID from Supabase Auth
                    Email = request.Email,
                    FullName = request.FullName,
                    CreatedAt = DateTime.UtcNow
                };

                try
                {
                    var profileResponse = await _supabase.Client.From<UserModel>().Insert(userProfile);
                    var createdProfile = profileResponse.Models?.FirstOrDefault();

                    if (createdProfile == null)
                    {
                        _logger.LogError("Failed to create user profile for {UserId}", userId);
                        throw new AppException("Failed to create user profile in database");
                    }

                    _logger.LogInformation("User profile created successfully for {UserId}", userId);

                    return new UserResponseDTO
                    {
                        Email = createdProfile.Email,
                        FullName = createdProfile.FullName,
                        UserId = createdProfile.Id,
                        AccessToken = authResult.AccessToken ?? string.Empty,
                        RefreshToken = authResult.RefreshToken ?? string.Empty
                    };
                }
                catch (Exception profileException)
                {
                    _logger.LogError(profileException, "Failed to create user profile for {UserId}. Error: {Error}", userId, profileException.Message);

                    // If it's a duplicate key error, try to find the existing profile
                    if (profileException.Message.Contains("duplicate key") || profileException.Message.Contains("23505"))
                    {
                        var existingProfileByEmail = await GetUserProfileByEmail(request.Email);
                        if (existingProfileByEmail != null)
                        {
                            _logger.LogInformation("Found existing profile with same email for {UserId}", userId);
                            return new UserResponseDTO
                            {
                                Email = existingProfileByEmail.Email,
                                FullName = existingProfileByEmail.FullName,
                                UserId = existingProfileByEmail.Id,
                                AccessToken = authResult.AccessToken ?? string.Empty,
                                RefreshToken = authResult.RefreshToken ?? string.Empty
                            };
                        }
                    }

                    // Cleanup: Try to sign out the user (since we can't delete them)
                    try
                    {
                        await _supabase.Client.Auth.SignOut();
                        _logger.LogInformation("Signed out user {UserId} after profile creation failure", userId);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(cleanupEx, "Failed to sign out user {UserId} after profile failure", userId);
                    }

                    throw new AppException($"Failed to create user profile: {profileException.Message}", profileException);
                }
            }
            catch (Exception ex) when (ex is not AppException && ex is not ValidationException)
            {
                _logger.LogError(ex, "Unexpected error during signup for {Email}", request.Email);

                // Check if this is a "user already registered" error from Supabase
                if (ex.Message.Contains("already registered") || ex.Message.Contains("already exists"))
                {
                    throw new AppException("An account with this email already exists. Please try logging in instead.");
                }

                throw new AppException("An unexpected error occurred during signup. Please try again.", ex);
            }
        }

        public async Task<UserResponseDTO> LoginAsync(LoginDTO request)
        {
            ValidationHelper.ValidateModel(request);

            _logger.LogInformation("Starting login process for email: {Email}", request.Email);

            try
            {
                // Step 1: Authenticate with Supabase Auth
                var session = await _supabase.Client.Auth.SignIn(request.Email, request.Password);

                if (session?.User == null)
                    throw new AppException("Invalid email or password.");

                var userId = Guid.Parse(session.User.Id!);
                _logger.LogInformation("User authenticated successfully: {UserId}", userId);

                // Step 2: Get user profile from our database
                var user = await GetOrCreateUserProfile(userId, request.Email, session.User.Email ?? request.Email);

                return new UserResponseDTO
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    UserId = user.Id,
                    AccessToken = session.AccessToken ?? string.Empty,
                    RefreshToken = session.RefreshToken ?? string.Empty
                };
            }
            catch (Exception ex) when (ex is not AppException && ex is not ValidationException)
            {
                _logger.LogError(ex, "Unexpected error during login for {Email}", request.Email);
                throw new AppException("Login failed. Please check your credentials and try again.", ex);
            }
        }

        private async Task<UserModel> GetOrCreateUserProfile(Guid userId, string email, string authEmail)
        {
            try
            {
                // First, try to find by user ID (most reliable)
                var response = await _supabase.Client.From<UserModel>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Get();

                var existingUser = response.Models?.FirstOrDefault();

                if (existingUser != null)
                {
                    _logger.LogInformation("Found existing user profile for {UserId}", userId);
                    return existingUser;
                }

                // If not found by ID, try by email (fallback for legacy users)
                var emailResponse = await _supabase.Client.From<UserModel>()
                    .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
                    .Get();

                existingUser = emailResponse.Models?.FirstOrDefault();

                if (existingUser != null)
                {
                    _logger.LogInformation("Found user profile by email for {UserId}, updating ID if needed", userId);

                    // Update the user's ID to match Auth ID if different
                    if (existingUser.Id != userId)
                    {
                        existingUser.Id = userId;
                        var updateResponse = await _supabase.Client.From<UserModel>().Update(existingUser);
                        return updateResponse.Models?.FirstOrDefault() ?? existingUser;
                    }

                    return existingUser;
                }

                // Profile doesn't exist, create it (recovery mechanism)
                _logger.LogWarning("User profile not found for authenticated user {UserId}, creating new profile", userId);

                var newProfile = new UserModel
                {
                    Id = userId,
                    Email = authEmail,
                    FullName = authEmail.Split('@')[0], // Use email prefix as default name
                    CreatedAt = DateTime.UtcNow
                };

                var createResponse = await _supabase.Client.From<UserModel>().Insert(newProfile);
                var createdProfile = createResponse.Models?.FirstOrDefault();

                if (createdProfile == null)
                    throw new AppException("Failed to create user profile during login recovery");

                _logger.LogInformation("Created recovery user profile for {UserId}", userId);
                return createdProfile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get or create user profile for {UserId}", userId);
                throw new NotFoundException($"User profile not found and could not be created: {ex.Message}");
            }
        }

        private async Task<UserModel?> GetUserProfileById(Guid userId)
        {
            try
            {
                var response = await _supabase.Client.From<UserModel>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Get();

                return response.Models?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for existing user profile {UserId}", userId);
                return null;
            }
        }

        private async Task<UserModel?> GetUserProfileByEmail(string email)
        {
            try
            {
                var response = await _supabase.Client.From<UserModel>()
                    .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
                    .Get();

                return response.Models?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for existing user profile with email {Email}", email);
                return null;
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                await _supabase.Client.Auth.SignOut();
                _logger.LogInformation("User logged out successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed.");
                return false;
            }
        }

        public async Task<UserResponseDTO?> RefreshTokenAsync()
        {
            var currentSession = _supabase.Client.Auth.CurrentSession;

            if (currentSession?.RefreshToken == null)
                throw new AppException("No refresh token available to refresh session.");

            _logger.LogInformation("Refreshing user session...");

            var refreshed = await _supabase.Client.Auth.RefreshSession();

            if (refreshed?.User == null)
                return null;

            var userId = Guid.Parse(refreshed.User.Id!);

            var user = await GetOrCreateUserProfile(userId, refreshed.User.Email ?? "", refreshed.User.Email ?? "");

            return new UserResponseDTO
            {
                Email = refreshed.User.Email ?? string.Empty,
                FullName = user.FullName,
                UserId = userId,
                AccessToken = refreshed.AccessToken ?? string.Empty,
                RefreshToken = refreshed.RefreshToken ?? string.Empty
            };
        }
    }
}
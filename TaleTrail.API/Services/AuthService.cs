using Supabase.Gotrue;
using TaleTrail.API.DTOs.Auth;
using TaleTrail.API.DTOs.Auth.Signup;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;
using TaleTrail.API.Models;

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

            _logger.LogInformation("Signing up user with email: {Email}", request.Email);

            var result = await _supabase.Client.Auth.SignUp(request.Email, request.Password);

            if (result?.User == null)
                throw new AppException("Signup failed. User was not created by Supabase.");

            var userProfile = new Models.User
            {
                Id = Guid.Parse(result.User.Id ?? throw new AppException("User ID missing")),
                Email = request.Email,
                FullName = request.FullName,
                CreatedAt = DateTime.UtcNow
            };

            await _supabase.Client.From<Models.User>().Insert(userProfile);

            return new UserResponseDTO
            {
                Email = userProfile.Email,
                FullName = userProfile.FullName,
                UserId = userProfile.Id,
                AccessToken = result.AccessToken ?? string.Empty,
                RefreshToken = result.RefreshToken ?? string.Empty
            };
        }

        public async Task<UserResponseDTO> LoginAsync(LoginDTO request)
        {
            ValidationHelper.ValidateModel(request);

            _logger.LogInformation("Logging in user with email: {Email}", request.Email);

            var session = await _supabase.Client.Auth.SignIn(request.Email, request.Password);

            if (session?.User == null)
                throw new AppException("Invalid email or password.");

            var userId = session.User.Id ?? throw new AppException("User ID missing from session.");

            var response = await _supabase.Client.From<Models.User>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId)
                .Get();

            var user = response.Models.FirstOrDefault()
                       ?? throw new NotFoundException("User profile not found in database.");

            return new UserResponseDTO
            {
                Email = user.Email,
                FullName = user.FullName,
                UserId = user.Id,
                AccessToken = session.AccessToken ?? string.Empty,
                RefreshToken = session.RefreshToken ?? string.Empty
            };
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

            var userId = refreshed.User.Id ?? throw new AppException("User ID missing after refresh.");

            var userResponse = await _supabase.Client.From<Models.User>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId)
                .Get();

            var user = userResponse.Models.FirstOrDefault();

            return new UserResponseDTO
            {
                Email = refreshed.User.Email ?? string.Empty,
                FullName = user?.FullName ?? string.Empty,
                UserId = Guid.Parse(userId),
                AccessToken = refreshed.AccessToken ?? string.Empty,
                RefreshToken = refreshed.RefreshToken ?? string.Empty
            };
        }
    }
}
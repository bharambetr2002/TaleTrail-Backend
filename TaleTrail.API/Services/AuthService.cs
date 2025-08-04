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
                throw new ValidationException("Invalid email format");

            var session = await _supabase.Client.Auth.SignUp(request.Email, request.Password);

            if (session?.User == null)
                throw new AppException("Signup failed - no user created");

            var user = new User
            {
                Id = Guid.Parse(session.User.Id),
                Email = request.Email,
                FullName = request.FullName,
                CreatedAt = DateTime.UtcNow
            };

            await _supabase.Client.From<User>().Insert(user);

            return new UserResponseDTO
            {
                Email = session.User.Email ?? request.Email,
                AccessToken = session.AccessToken ?? string.Empty,
                RefreshToken = session.RefreshToken ?? string.Empty,
                UserId = Guid.Parse(session.User.Id),
                FullName = request.FullName
            };
        }

        public async Task<UserResponseDTO> LoginAsync(LoginDTO request)
        {
            ValidationHelper.ValidateModel(request);

            var session = await _supabase.Client.Auth.SignIn(request.Email, request.Password);

            if (session?.User == null)
                throw new AppException("Invalid email or password");

            var userResponse = await _supabase.Client.From<User>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, session.User.Id)
                .Get();

            var user = userResponse.Models.FirstOrDefault();

            return new UserResponseDTO
            {
                Email = session.User.Email ?? request.Email,
                AccessToken = session.AccessToken ?? string.Empty,
                RefreshToken = session.RefreshToken ?? string.Empty,
                UserId = Guid.Parse(session.User.Id),
                FullName = user?.FullName
            };
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                await _supabase.Client.Auth.SignOut();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                return false;
            }
        }

        public async Task<UserResponseDTO?> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var session = await _supabase.Client.Auth.RefreshSession(refreshToken);

                if (session?.User == null)
                    return null;

                var userResponse = await _supabase.Client.From<User>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, session.User.Id)
                    .Get();

                var user = userResponse.Models.FirstOrDefault();

                return new UserResponseDTO
                {
                    Email = session.User.Email ?? string.Empty,
                    AccessToken = session.AccessToken ?? string.Empty,
                    RefreshToken = session.RefreshToken ?? string.Empty,
                    UserId = Guid.Parse(session.User.Id),
                    FullName = user?.FullName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh failed");
                return null;
            }
        }
    }
}
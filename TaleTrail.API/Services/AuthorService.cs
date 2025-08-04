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

        public AuthService(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        public async Task<UserResponseDTO> SignupAsync(SignupDTO request)
        {
            ValidationHelper.ValidateModel(request);

            if (!ValidationHelper.IsValidEmail(request.Email))
                throw new ValidationException("Invalid email format");

            try
            {
                var session = await _supabase.Client.Auth.SignUp(request.Email, request.Password);

                if (session?.User == null)
                    throw new AppException("Signup failed");

                // Create user profile
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
            catch (Exception ex)
            {
                throw new AppException($"Signup failed: {ex.Message}");
            }
        }

        public async Task<UserResponseDTO> LoginAsync(LoginDTO request)
        {
            ValidationHelper.ValidateModel(request);

            try
            {
                var session = await _supabase.Client.Auth.SignIn(request.Email, request.Password);

                if (session?.User == null)
                    throw new AppException("Invalid credentials");

                // Get user profile
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
            catch (Exception ex)
            {
                throw new AppException($"Login failed: {ex.Message}");
            }
        }
    }
}
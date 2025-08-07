using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Services;
using Supabase.Gotrue;

namespace TaleTrail.API.Services;

public class AuthService
{
    private readonly SupabaseService _supabaseService;

    public AuthService(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<AuthResponseDTO> SignUpAsync(SignupRequestDTO request)
    {
        try
        {
            var session = await _supabaseService.Supabase.Auth.SignUp(request.Email, request.Password, new SignUpOptions
            {
                Data = new Dictionary<string, object>
                {
                    { "full_name", request.FullName },
                    { "username", request.Username }
                }
            });

            if (session?.User == null)
                throw new InvalidOperationException("Failed to create account");

            return new AuthResponseDTO
            {
                AccessToken = session.AccessToken ?? string.Empty,
                RefreshToken = session.RefreshToken ?? string.Empty,
                User = new UserResponseDTO
                {
                    Id = Guid.Parse(session.User.Id),
                    Email = session.User.Email ?? string.Empty,
                    FullName = request.FullName,
                    Username = request.Username,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Signup failed: {ex.Message}", ex);
        }
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        try
        {
            var session = await _supabaseService.Supabase.Auth.SignIn(request.Email, request.Password);

            if (session?.User == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            return new AuthResponseDTO
            {
                AccessToken = session.AccessToken ?? string.Empty,
                RefreshToken = session.RefreshToken ?? string.Empty,
                User = new UserResponseDTO
                {
                    Id = Guid.Parse(session.User.Id),
                    Email = session.User.Email ?? string.Empty,
                    FullName = session.User.UserMetadata?["full_name"]?.ToString() ?? string.Empty,
                    Username = session.User.UserMetadata?["username"]?.ToString() ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Login failed: {ex.Message}", ex);
        }
    }
}
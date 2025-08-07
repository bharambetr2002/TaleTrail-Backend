using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using Supabase.Gotrue;

namespace TaleTrail.API.Services;

public class AuthService
{
    private readonly SupabaseService _supabaseService;

    public AuthService(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<AuthResponse> SignUpAsync(SignupRequest request)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required");

            if (string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username is required");

            var session = await _supabaseService.Supabase.Auth.SignUp(request.Email, request.Password, new SignUpOptions
            {
                Data = new Dictionary<string, object>
                {
                    { "full_name", request.FullName ?? "" },
                    { "username", request.Username }
                }
            });

            if (session?.User == null)
                throw new InvalidOperationException("Failed to create user account");

            return new AuthResponse
            {
                AccessToken = session.AccessToken ?? "",
                RefreshToken = session.RefreshToken ?? "",
                User = new Model.User
                {
                    Id = Guid.Parse(session.User.Id),
                    Email = session.User.Email ?? "",
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

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required");

            var session = await _supabaseService.Supabase.Auth.SignIn(request.Email, request.Password);

            if (session?.User == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Note: We don't create user profile here - that's handled by BaseController's "self-healing" logic
            // This keeps the AuthService focused only on authentication

            return new AuthResponse
            {
                AccessToken = session.AccessToken ?? "",
                RefreshToken = session.RefreshToken ?? "",
                User = new Model.User
                {
                    Id = Guid.Parse(session.User.Id),
                    Email = session.User.Email ?? "",
                    FullName = session.User.UserMetadata?["full_name"]?.ToString() ?? "",
                    Username = session.User.UserMetadata?["username"]?.ToString() ?? "",
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

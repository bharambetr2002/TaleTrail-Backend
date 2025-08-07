using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.DAO;
using Supabase.Gotrue;

namespace TaleTrail.API.Services;

public class AuthService
{
    private readonly SupabaseService _supabaseService;
    private readonly UserDao _userDao;

    public AuthService(SupabaseService supabaseService, UserDao userDao)
    {
        _supabaseService = supabaseService;
        _userDao = userDao;
    }

    public async Task<AuthResponse> SignUpAsync(SignupRequest request)
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
                throw new Exception("Failed to create user");

            return new AuthResponse
            {
                AccessToken = session.AccessToken ?? "",
                RefreshToken = session.RefreshToken ?? "",
                User = new User
                {
                    Id = Guid.Parse(session.User.Id),
                    Email = session.User.Email ?? "",
                    FullName = request.FullName,
                    Username = request.Username
                }
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Signup failed: {ex.Message}");
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var session = await _supabaseService.Supabase.Auth.SignIn(request.Email, request.Password);

            if (session?.User == null)
                throw new Exception("Invalid credentials");

            // Get or create user profile
            var userId = Guid.Parse(session.User.Id);
            var user = await _userDao.GetByIdAsync(userId);

            if (user == null)
            {
                // Create user profile from auth data
                user = new User
                {
                    Id = userId,
                    Email = session.User.Email ?? "",
                    FullName = session.User.UserMetadata?["full_name"]?.ToString() ?? "",
                    Username = session.User.UserMetadata?["username"]?.ToString() ?? "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                user = await _userDao.CreateAsync(user);
            }

            return new AuthResponse
            {
                AccessToken = session.AccessToken ?? "",
                RefreshToken = session.RefreshToken ?? "",
                User = user
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Login failed: {ex.Message}");
        }
    }
}
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Services;
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

            var userId = Guid.Parse(session.User.Id ?? throw new InvalidOperationException("User ID is null"));

            TaleTrail.API.Model.User createdUser;
            try
            {
                var result = await _supabaseService.Supabase.Rpc("create_user_profile", new Dictionary<string, object>
                {
                    { "user_id", userId },
                    { "user_email", request.Email },
                    { "full_name", request.FullName },
                    { "username", request.Username }
                });

                createdUser = new TaleTrail.API.Model.User
                {
                    Id = userId,
                    Email = request.Email,
                    FullName = request.FullName,
                    Username = request.Username,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }
            catch (Exception)
            {
                var userProfile = new TaleTrail.API.Model.User
                {
                    Id = userId,
                    Email = request.Email,
                    FullName = request.FullName,
                    Username = request.Username,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                createdUser = await _userDao.CreateAsync(userProfile);
            }

            return new AuthResponseDTO
            {
                AccessToken = session.AccessToken ?? string.Empty,
                RefreshToken = session.RefreshToken ?? string.Empty,
                User = new UserResponseDTO
                {
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    FullName = createdUser.FullName ?? string.Empty,
                    Username = createdUser.Username,
                    Bio = createdUser.Bio,
                    AvatarUrl = createdUser.AvatarUrl,
                    CreatedAt = createdUser.CreatedAt,
                    UpdatedAt = createdUser.UpdatedAt
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

            var userId = Guid.Parse(session.User.Id ?? throw new InvalidOperationException("User ID is null"));


            var userProfile = await _userDao.GetByIdAsync(userId);
            if (userProfile == null)
            {
                userProfile = new TaleTrail.API.Model.User
                {
                    Id = userId,
                    Email = session.User.Email ?? string.Empty,
                    FullName = session.User.UserMetadata?["full_name"]?.ToString() ?? string.Empty,
                    Username = session.User.UserMetadata?["username"]?.ToString() ?? session.User.Email?.Split('@')[0] ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                userProfile = await _userDao.CreateAsync(userProfile);
            }

            return new AuthResponseDTO
            {
                AccessToken = session.AccessToken ?? string.Empty,
                RefreshToken = session.RefreshToken ?? string.Empty,
                User = new UserResponseDTO
                {
                    Id = userProfile.Id,
                    Email = userProfile.Email,
                    FullName = userProfile.FullName ?? string.Empty,
                    Username = userProfile.Username,
                    Bio = userProfile.Bio,
                    AvatarUrl = userProfile.AvatarUrl,
                    CreatedAt = userProfile.CreatedAt,
                    UpdatedAt = userProfile.UpdatedAt
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
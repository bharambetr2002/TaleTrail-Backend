using TaleTrail.API.DTOs.Auth;

namespace TaleTrail.API.Services.Interfaces
{
    public interface ISupabaseAuthService
    {
        Task<object> SignUpAsync(AuthRequestDto dto);
        Task<object> SignInAsync(AuthRequestDto dto);
        Task<UserDto?> GetUserFromToken(string token);
    }
}
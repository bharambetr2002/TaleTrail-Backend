namespace TaleTrail.API.DTOs.Auth
{
    public class UserResponseDTO
    {
        public string Email { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string? FullName { get; set; }
    }
}
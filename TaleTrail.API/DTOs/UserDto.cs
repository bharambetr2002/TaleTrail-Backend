namespace TaleTrail.API.DTOs
{
    public class UserDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
    }
}
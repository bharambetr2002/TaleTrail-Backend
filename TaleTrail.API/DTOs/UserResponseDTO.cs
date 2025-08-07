using System;

namespace TaleTrail.API.DTOs
{
    // This is a clean data object. It has no complex attributes,
    // so the JSON serializer can handle it without any problems.
    public class UserResponseDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Location { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
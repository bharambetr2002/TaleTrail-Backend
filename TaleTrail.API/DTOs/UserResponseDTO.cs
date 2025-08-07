using System;

namespace TaleTrail.API.DTOs
{
    public class UserResponseDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty; // Add default value
        public string Email { get; set; } = string.Empty; // Add default value
        public string FullName { get; set; } = string.Empty; // Add default value
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
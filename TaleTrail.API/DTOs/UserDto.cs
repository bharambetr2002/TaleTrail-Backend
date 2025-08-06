// File: TaleTrail.API/DTOs/UserDto.cs
using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.DTOs
{
    public class UserDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string? Bio { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? AvatarUrl { get; set; }

        // Optional: include if needed in admin panel logic
        public bool IsAdmin { get; set; } = false;

        // Optional: include if manually creating users (e.g., from backend)
        public Guid? Id { get; set; }
    }
}
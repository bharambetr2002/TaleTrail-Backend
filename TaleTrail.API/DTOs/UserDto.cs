using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.DTOs
{
    public class UserDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string? Bio { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? AvatarUrl { get; set; }

        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string? Location { get; set; }
    }
}
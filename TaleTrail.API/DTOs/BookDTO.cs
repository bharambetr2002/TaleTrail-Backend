using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.DTOs
{
    public class BookDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? CoverUrl { get; set; }

        [StringLength(50, ErrorMessage = "Language cannot exceed 50 characters")]
        public string? Language { get; set; }

        [Range(1000, 9999, ErrorMessage = "Publication year must be between 1000 and 9999")]
        public int? PublicationYear { get; set; }
    }
}
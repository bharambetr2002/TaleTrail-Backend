using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.DTOs
{
    public class BlogDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
        public string? Content { get; set; }
    }
}

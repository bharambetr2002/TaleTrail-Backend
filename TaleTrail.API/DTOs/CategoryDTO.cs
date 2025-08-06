using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.DTOs
{
    public class CategoryDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;
    }
}
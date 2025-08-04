using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.DTOs
{
    public class WatchlistDto
    {
        [Required(ErrorMessage = "Book ID is required")]
        public Guid BookId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(to_read|reading|completed|dropped)$",
            ErrorMessage = "Status must be one of: to_read, reading, completed, dropped")]
        public string Status { get; set; } = string.Empty;
    }
}
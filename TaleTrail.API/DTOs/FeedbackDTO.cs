using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.DTOs
{
    public class FeedbackDto
    {
        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string Message { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.Model.DTOs;

public class CreateReviewRequest
{
    [Required]
    public Guid BookId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [MaxLength(2000, ErrorMessage = "Review content cannot exceed 2000 characters")]
    public string? Content { get; set; }
}

public class UpdateReviewRequest
{
    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [MaxLength(2000, ErrorMessage = "Review content cannot exceed 2000 characters")]
    public string? Content { get; set; }
}
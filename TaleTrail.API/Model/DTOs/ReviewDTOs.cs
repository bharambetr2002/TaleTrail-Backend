using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.Model.DTOs;

public class ReviewResponseDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewRequestDTO
{
    [Required]
    public Guid BookId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(2000)]
    public string? Content { get; set; }
}

public class UpdateReviewRequestDTO
{
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(2000)]
    public string? Content { get; set; }
}
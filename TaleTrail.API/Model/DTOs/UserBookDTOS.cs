using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.Model.DTOs;

public class UserBookResponseDTO
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string? BookCoverUrl { get; set; }
    public ReadingStatus ReadingStatus { get; set; }
    public int Progress { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime AddedAt { get; set; }
}

public class AddUserBookRequestDTO
{
    [Required]
    public Guid BookId { get; set; }

    [Required]
    public ReadingStatus ReadingStatus { get; set; }

    [Range(0, 100)]
    public int Progress { get; set; } = 0;
}

public class UpdateUserBookRequestDTO
{
    [Required]
    public ReadingStatus ReadingStatus { get; set; }

    [Range(0, 100)]
    public int Progress { get; set; }
}

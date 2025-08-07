using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.Model.DTOs;

public class AddUserBookRequest
{
    [Required]
    public Guid BookId { get; set; }

    [Required]
    public ReadingStatus ReadingStatus { get; set; }

    [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100")]
    public int Progress { get; set; } = 0;
}

public class UpdateUserBookRequest
{
    [Required]
    public ReadingStatus ReadingStatus { get; set; }

    [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100")]
    public int Progress { get; set; }
}

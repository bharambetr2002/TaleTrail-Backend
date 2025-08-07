namespace TaleTrail.API.Model.DTOs;

public class CreateReviewRequest
{
    public Guid BookId { get; set; }
    public int Rating { get; set; }
    public string? Content { get; set; }
}

public class UpdateReviewRequest
{
    public int Rating { get; set; }
    public string? Content { get; set; }
}
namespace TaleTrail.API.Model.DTOs;

public class AddUserBookRequest
{
    public Guid BookId { get; set; }
    public ReadingStatus ReadingStatus { get; set; }
    public int Progress { get; set; } = 0;
}

public class UpdateUserBookRequest
{
    public ReadingStatus ReadingStatus { get; set; }
    public int Progress { get; set; }
}
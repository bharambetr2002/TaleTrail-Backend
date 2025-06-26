using TaleTrail.API.Enums;

public class Watchlist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public ReadingStatus Status { get; set; } // Enum

    public User? User { get; set; }
    public Book? Book { get; set; }
}

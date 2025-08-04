namespace TaleTrail.API.DTOs
{
    public class WatchlistDto
    {
        public Guid BookId { get; set; }
        public string Status { get; set; } // e.g., to_read, reading, completed
    }
}
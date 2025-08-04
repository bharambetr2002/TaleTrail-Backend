namespace TaleTrail.API.DTOs
{
    public class ReviewDto
    {
        public Guid BookId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
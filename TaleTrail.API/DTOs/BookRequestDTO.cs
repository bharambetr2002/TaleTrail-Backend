namespace TaleTrail.API.DTOs
{
    public class BookRequestDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ISBN { get; set; }
        public string? CoverImageUrl { get; set; }
        public List<int>? AuthorIds { get; set; }
        public int PublisherId { get; set; }
        public List<int>? CategoryIds { get; set; }
    }
}

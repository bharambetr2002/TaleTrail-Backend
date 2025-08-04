namespace TaleTrail.API.DTOs
{
    public class BookDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? CoverUrl { get; set; }
        public string? Language { get; set; }
        public int? PublicationYear { get; set; }
    }
}
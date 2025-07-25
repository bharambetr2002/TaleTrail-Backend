namespace TaleTrail.API.DTOs
{
    public class BookResponseDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ISBN { get; set; }
        public string? CoverImageUrl { get; set; }
        public List<string>? Authors { get; set; }
        public string? Publisher { get; set; }
        public List<string>? Categories { get; set; }
    }
}

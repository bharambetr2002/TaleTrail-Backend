namespace TaleTrail.API.DTOs
{
    public class BookResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverUrl { get; set; }
        public string? Language { get; set; }
        public int? PublicationYear { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public static class BookExtensions
    {
        public static BookResponseDto ToDto(this TaleTrail.API.Models.Book book)
        {
            return new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                CoverUrl = book.CoverUrl,
                Language = book.Language,
                PublicationYear = book.PublicationYear,
                UserId = book.UserId,
                CreatedAt = book.CreatedAt
            };
        }

        public static List<BookResponseDto> ToDto(this List<TaleTrail.API.Models.Book> books)
        {
            return books.Select(book => book.ToDto()).ToList();
        }
    }
}
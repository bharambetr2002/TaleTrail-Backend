public class Book
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? ISBN { get; set; }
    public string? Summary { get; set; }
    public string? CoverUrl { get; set; }
    public double AvgRating { get; set; }

    public ICollection<BookAuthor>? BookAuthors { get; set; }
    public ICollection<BookPublisher>? BookPublishers { get; set; }
    public ICollection<BookCategory>? BookCategories { get; set; }

    public ICollection<Watchlist>? Watchlists { get; set; }
    public ICollection<Review>? Reviews { get; set; }
}

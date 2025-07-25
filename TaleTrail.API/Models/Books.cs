using System.Collections.Generic;

namespace TaleTrail.API.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public string CoverImageUrl { get; set; }

        // Navigation properties
        public List<Author> Authors { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public Publisher Publisher { get; set; }
        public int? PublisherId { get; set; }

        public List<Review> Reviews { get; set; } = new();
        public List<Watchlist> WatchlistedBy { get; set; } = new();

        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; } = new Publisher();
    }
}
namespace TaleTrail.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property for many-to-many with Book
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}
namespace TaleTrail.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Role { get; set; } // "User", "Admin", "Subscriber"
        public string? City { get; set; }

        public ICollection<Watchlist>? Watchlist { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Blog>? Blogs { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
    }
}
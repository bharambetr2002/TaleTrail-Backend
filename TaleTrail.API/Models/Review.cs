namespace TaleTrail.API.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string? Text { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
        public Book? Book { get; set; }
    }
}
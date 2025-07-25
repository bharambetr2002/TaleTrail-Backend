namespace TaleTrail.API.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
    }
}
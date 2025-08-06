namespace TaleTrail.API.DTOs.Profile
{
    public class UserStatsDto
    {
        public int BooksCompleted { get; set; }
        public int CurrentlyReading { get; set; }
        public int WishlistCount { get; set; }
        public double AverageRating { get; set; }
        public int BlogsWritten { get; set; }
    }
}
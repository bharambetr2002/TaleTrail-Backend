using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Services
{
    public class ReviewService
    {
        private readonly SupabaseService _supabase;

        public ReviewService(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        public async Task<List<Review>> GetBookReviewsAsync(Guid bookId)
        {
            var response = await _supabase.Client.From<Review>()
                .Filter("book_id", Supabase.Postgrest.Constants.Operator.Equals, bookId.ToString())
                .Get();
            return response.Models;
        }

        public async Task<Review> CreateReviewAsync(ReviewDto reviewDto, Guid userId)
        {
            ValidationHelper.ValidateModel(reviewDto);

            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
                throw new ValidationException("Rating must be between 1 and 5");

            var review = new Review
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = reviewDto.BookId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            var response = await _supabase.Client.From<Review>().Insert(review);
            return response.Models.First();
        }
    }
}

using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.DAO;

namespace TaleTrail.API.Services;

public class ReviewService
{
    private readonly ReviewDao _reviewDao;

    public ReviewService(ReviewDao reviewDao)
    {
        _reviewDao = reviewDao;
    }

    public async Task<List<Review>> GetReviewsByBookIdAsync(Guid bookId)
    {
        return await _reviewDao.GetByBookIdAsync(bookId);
    }

    public async Task<List<Review>> GetReviewsByUserIdAsync(Guid userId)
    {
        return await _reviewDao.GetByUserIdAsync(userId);
    }

    public async Task<Review?> GetReviewByIdAsync(Guid id)
    {
        return await _reviewDao.GetByIdAsync(id);
    }

    public async Task<Review> CreateReviewAsync(Guid userId, CreateReviewRequest request)
    {
        // Validate rating
        if (request.Rating < 1 || request.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookId = request.BookId,
            Rating = request.Rating,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _reviewDao.CreateAsync(review);
    }

    public async Task<Review> UpdateReviewAsync(Guid reviewId, Guid userId, UpdateReviewRequest request)
    {
        var existingReview = await _reviewDao.GetByIdAsync(reviewId);
        if (existingReview == null)
            throw new Exception("Review not found");

        if (existingReview.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own reviews");

        // Validate rating
        if (request.Rating < 1 || request.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5");

        existingReview.Rating = request.Rating;
        existingReview.Content = request.Content;

        return await _reviewDao.UpdateAsync(existingReview);
    }

    public async Task DeleteReviewAsync(Guid reviewId, Guid userId)
    {
        var existingReview = await _reviewDao.GetByIdAsync(reviewId);
        if (existingReview == null)
            throw new Exception("Review not found");

        if (existingReview.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own reviews");

        await _reviewDao.DeleteAsync(reviewId);
    }
}
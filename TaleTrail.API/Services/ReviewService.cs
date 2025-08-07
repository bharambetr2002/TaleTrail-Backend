using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.DAO;

namespace TaleTrail.API.Services;

public class ReviewService
{
    private readonly ReviewDao _reviewDao;
    private readonly BookDao _bookDao;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(ReviewDao reviewDao, BookDao bookDao, ILogger<ReviewService> logger)
    {
        _reviewDao = reviewDao;
        _bookDao = bookDao;
        _logger = logger;
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

        // CRITICAL FIX: Validate book exists
        var book = await _bookDao.GetByIdAsync(request.BookId);
        if (book == null)
            throw new KeyNotFoundException($"Book with ID {request.BookId} not found");

        // Check if user already reviewed this book (prevent duplicate reviews)
        var existingReviews = await _reviewDao.GetByBookIdAsync(request.BookId);
        var existingUserReview = existingReviews.FirstOrDefault(r => r.UserId == userId);
        if (existingUserReview != null)
            throw new InvalidOperationException("You have already reviewed this book. Use update instead.");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookId = request.BookId,
            Rating = request.Rating,
            Content = request.Content?.Trim(), // Trim whitespace
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        try
        {
            return await _reviewDao.CreateAsync(review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create review for book {BookId} by user {UserId}", request.BookId, userId);
            throw new InvalidOperationException("Failed to create review", ex);
        }
    }

    public async Task<Review> UpdateReviewAsync(Guid reviewId, Guid userId, UpdateReviewRequest request)
    {
        var existingReview = await _reviewDao.GetByIdAsync(reviewId);
        if (existingReview == null)
            throw new KeyNotFoundException("Review not found");

        if (existingReview.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own reviews");

        // Validate rating
        if (request.Rating < 1 || request.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5");

        existingReview.Rating = request.Rating;
        existingReview.Content = request.Content?.Trim(); // Trim whitespace

        try
        {
            return await _reviewDao.UpdateAsync(existingReview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update review {ReviewId}", reviewId);
            throw new InvalidOperationException("Failed to update review", ex);
        }
    }

    public async Task DeleteReviewAsync(Guid reviewId, Guid userId)
    {
        var existingReview = await _reviewDao.GetByIdAsync(reviewId);
        if (existingReview == null)
            throw new KeyNotFoundException("Review not found");

        if (existingReview.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own reviews");

        try
        {
            await _reviewDao.DeleteAsync(reviewId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete review {ReviewId}", reviewId);
            throw new InvalidOperationException("Failed to delete review", ex);
        }
    }
}
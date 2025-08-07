using TaleTrail.API.DAO;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Model;

namespace TaleTrail.API.Services;

public class ReviewService
{
    private readonly ReviewDao _reviewDao;
    private readonly BookDao _bookDao;
    private readonly UserDao _userDao;

    public ReviewService(ReviewDao reviewDao, BookDao bookDao, UserDao userDao)
    {
        _reviewDao = reviewDao;
        _bookDao = bookDao;
        _userDao = userDao;
    }

    public async Task<List<ReviewResponseDTO>> GetReviewsByBookIdAsync(Guid bookId)
    {
        var reviews = await _reviewDao.GetByBookIdAsync(bookId);
        var reviewDtos = new List<ReviewResponseDTO>();

        foreach (var review in reviews)
        {
            var reviewDto = await MapToReviewResponseDTO(review);
            reviewDtos.Add(reviewDto);
        }

        return reviewDtos;
    }

    public async Task<ReviewResponseDTO> CreateReviewAsync(Guid userId, CreateReviewRequestDTO request)
    {
        // Validate book exists
        var book = await _bookDao.GetByIdAsync(request.BookId);
        if (book == null)
            throw new KeyNotFoundException("Book not found");

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

        var createdReview = await _reviewDao.CreateAsync(review);
        return await MapToReviewResponseDTO(createdReview);
    }

    public async Task<ReviewResponseDTO> UpdateReviewAsync(Guid reviewId, Guid userId, UpdateReviewRequestDTO request)
    {
        var review = await _reviewDao.GetByIdAsync(reviewId);
        if (review == null)
            throw new KeyNotFoundException("Review not found");

        if (review.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own reviews");

        review.Rating = request.Rating;
        review.Content = request.Content;
        review.UpdatedAt = DateTime.UtcNow;

        var updatedReview = await _reviewDao.UpdateAsync(review);
        return await MapToReviewResponseDTO(updatedReview);
    }

    public async Task DeleteReviewAsync(Guid reviewId, Guid userId)
    {
        var review = await _reviewDao.GetByIdAsync(reviewId);
        if (review == null)
            throw new KeyNotFoundException("Review not found");

        if (review.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own reviews");

        await _reviewDao.DeleteAsync(reviewId);
    }

    private async Task<ReviewResponseDTO> MapToReviewResponseDTO(Review review)
    {
        var user = await _userDao.GetByIdAsync(review.UserId);
        var book = await _bookDao.GetByIdAsync(review.BookId);

        return new ReviewResponseDTO
        {
            Id = review.Id,
            UserId = review.UserId,
            Username = user?.Username ?? "Unknown User",
            BookId = review.BookId,
            BookTitle = book?.Title ?? "Unknown Book",
            Rating = review.Rating,
            Content = review.Content,
            CreatedAt = review.CreatedAt
        };
    }
}
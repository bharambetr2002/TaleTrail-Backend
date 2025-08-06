using TaleTrail.API.DAO;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.Services
{
    public class ReviewService
    {
        private readonly ReviewDao _reviewDao;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(ReviewDao reviewDao, ILogger<ReviewService> logger)
        {
            _reviewDao = reviewDao;
            _logger = logger;
        }

        public async Task<List<Review>> GetReviewsForBookAsync(Guid bookId)
        {
            return await _reviewDao.GetByBookIdAsync(bookId);
        }

        public async Task<List<Review>> GetReviewsByUserAsync(Guid userId)
        {
            return await _reviewDao.GetByUserIdAsync(userId);
        }

        public async Task<Review> CreateReviewAsync(ReviewDto reviewDto, Guid userId)
        {
            var review = new Review
            {
                UserId = userId,
                BookId = reviewDto.BookId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            var createdReview = await _reviewDao.AddAsync(review);
            if (createdReview == null)
            {
                throw new AppException("Failed to create review.");
            }
            return createdReview;
        }

        public async Task<Review?> UpdateReviewAsync(Guid id, ReviewDto reviewDto, Guid userId)
        {
            var existingReview = await _reviewDao.GetByIdAsync(id);
            if (existingReview == null)
            {
                return null; // Not found
            }

            // Authorization check: Only allow the owner to update
            if (existingReview.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to update review {ReviewId} owned by {OwnerId}", userId, id, existingReview.UserId);
                return null;
            }

            existingReview.Rating = reviewDto.Rating;
            existingReview.Comment = reviewDto.Comment;

            return await _reviewDao.UpdateAsync(existingReview);
        }

        public async Task<bool> DeleteReviewAsync(Guid id, Guid userId)
        {
            var existingReview = await _reviewDao.GetByIdAsync(id);
            if (existingReview == null)
            {
                return false; // Not found
            }

            // Authorization check: Only allow the owner to delete
            if (existingReview.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to delete review {ReviewId} owned by {OwnerId}", userId, id, existingReview.UserId);
                return false;
            }

            await _reviewDao.DeleteAsync(id);
            return true;
        }
    }
}

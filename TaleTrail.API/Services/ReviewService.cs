using TaleTrail.API.DAO;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaleTrail.API.Services
{
    public class ReviewService
    {
        private readonly ReviewDao _reviewDao;

        public ReviewService(ReviewDao reviewDao)
        {
            _reviewDao = reviewDao;
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
            if (existingReview == null || existingReview.UserId != userId)
            {
                return null;
            }
            existingReview.Rating = reviewDto.Rating;
            existingReview.Comment = reviewDto.Comment;
            return await _reviewDao.UpdateAsync(existingReview);
        }

        public async Task<bool> DeleteReviewAsync(Guid id, Guid userId)
        {
            var existingReview = await _reviewDao.GetByIdAsync(id);
            if (existingReview == null || existingReview.UserId != userId)
            {
                return false;
            }
            await _reviewDao.DeleteAsync(id);
            return true;
        }
    }
}
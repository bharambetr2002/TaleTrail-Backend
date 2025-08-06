using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : BaseController
    {
        private readonly ReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(ReviewService reviewService, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetReviewsForBook(Guid bookId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsForBookAsync(bookId);
                return Ok(ApiResponse<object>.SuccessResult(reviews));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for book {BookId}", bookId);
                return BadRequest(ApiResponse.ErrorResult($"Error getting book reviews: {ex.Message}"));
            }
        }

        [HttpGet("my-reviews")]
        [Authorize]
        public async Task<IActionResult> GetMyReviews()
        {
            try
            {
                var userId = GetCurrentUserId();
                var reviews = await _reviewService.GetReviewsByUserAsync(userId);
                return Ok(ApiResponse<object>.SuccessResult(reviews));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user's reviews");
                return BadRequest(ApiResponse.ErrorResult($"Error getting your reviews: {ex.Message}"));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDto reviewDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var createdReview = await _reviewService.CreateReviewAsync(reviewDto, userId);
                return Ok(ApiResponse<object>.SuccessResult(createdReview, "Review created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return BadRequest(ApiResponse.ErrorResult($"Error creating review: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(Guid id, [FromBody] ReviewDto reviewDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var updatedReview = await _reviewService.UpdateReviewAsync(id, reviewDto, userId);

                if (updatedReview == null)
                {
                    // CORRECTED LINE: Pass the error message string directly to Forbid()
                    return Forbid("You are not authorized to update this review or it does not exist.");
                }

                return Ok(ApiResponse<object>.SuccessResult(updatedReview, "Review updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error updating review: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _reviewService.DeleteReviewAsync(id, userId);

                if (!success)
                {
                    // CORRECTED LINE: Pass the error message string directly to Forbid()
                    return Forbid("You are not authorized to delete this review or it does not exist.");
                }

                return Ok(ApiResponse.SuccessResult("Review deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error deleting review: {ex.Message}"));
            }
        }
    }
}
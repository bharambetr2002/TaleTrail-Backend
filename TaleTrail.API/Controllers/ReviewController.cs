using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : BaseController
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(SupabaseService supabase, ILogger<ReviewController> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _supabase.Client.From<Review>().Get();
                var reviews = response.Models?.Select(r => new
                {
                    r.Id,
                    r.UserId,
                    r.BookId,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(reviews, $"Found {reviews?.Count ?? 0} reviews"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all reviews");
                return BadRequest(ApiResponse.ErrorResult($"Error getting reviews: {ex.Message}"));
            }
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetByBook(Guid bookId)
        {
            try
            {
                var response = await _supabase.Client
                    .From<Review>()
                    .Filter("book_id", Supabase.Postgrest.Constants.Operator.Equals, bookId.ToString())
                    .Get();

                var reviews = response.Models?.Select(r => new
                {
                    r.Id,
                    r.UserId,
                    r.BookId,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(reviews, $"Found {reviews?.Count ?? 0} reviews for book"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for book {BookId}", bookId);
                return BadRequest(ApiResponse.ErrorResult($"Error getting book reviews: {ex.Message}"));
            }
        }

        [HttpGet("user/my-reviews")]
        public async Task<IActionResult> GetMyReviews()
        {
            try
            {
                var userId = GetCurrentUserId();

                var response = await _supabase.Client
                    .From<Review>()
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Get();

                var reviews = response.Models?.Select(r => new
                {
                    r.Id,
                    r.UserId,
                    r.BookId,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(reviews, $"Found {reviews?.Count ?? 0} reviews"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to get user reviews");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user reviews");
                return BadRequest(ApiResponse.ErrorResult($"Error getting user reviews: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReviewDto reviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                // Get user ID from JWT token via middleware
                var userId = GetCurrentUserId();

                var review = new Review
                {
                    Id = Guid.NewGuid(),
                    UserId = userId, // From JWT token, not from client
                    BookId = reviewDto.BookId,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                var response = await _supabase.Client.From<Review>().Insert(review);
                var created = response.Models?.FirstOrDefault();

                if (created == null)
                    return BadRequest(ApiResponse.ErrorResult("Failed to create review"));

                var result = new
                {
                    created.Id,
                    created.UserId,
                    created.BookId,
                    created.Rating,
                    created.Comment,
                    created.CreatedAt
                };

                return Ok(ApiResponse<object>.SuccessResult(result, "Review created successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized review creation attempt");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return BadRequest(ApiResponse.ErrorResult($"Error creating review: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ReviewDto reviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var userId = GetCurrentUserId();

                // Get existing review and verify ownership
                var existingResponse = await _supabase.Client
                    .From<Review>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                var existingReview = existingResponse.Models?.FirstOrDefault();
                if (existingReview == null)
                    return NotFound(ApiResponse.ErrorResult("Review not found"));

                if (existingReview.UserId != userId)
                    return Forbid("You can only update your own reviews");

                var updated = new Review
                {
                    Id = id,
                    UserId = userId,
                    BookId = reviewDto.BookId,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment,
                    CreatedAt = existingReview.CreatedAt // Keep original creation date
                };

                var response = await _supabase.Client.From<Review>().Update(updated);
                var review = response.Models?.FirstOrDefault();

                if (review == null)
                    return BadRequest(ApiResponse.ErrorResult("Update failed"));

                var result = new
                {
                    review.Id,
                    review.UserId,
                    review.BookId,
                    review.Rating,
                    review.Comment,
                    review.CreatedAt
                };

                return Ok(ApiResponse<object>.SuccessResult(result, "Review updated successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized review update attempt for review {ReviewId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error updating review: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Get existing review and verify ownership
                var existingResponse = await _supabase.Client
                    .From<Review>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                var existingReview = existingResponse.Models?.FirstOrDefault();
                if (existingReview == null)
                    return NotFound(ApiResponse.ErrorResult("Review not found"));

                if (existingReview.UserId != userId)
                    return Forbid("You can only delete your own reviews");

                await _supabase.Client.From<Review>().Delete(existingReview);

                return Ok(ApiResponse.SuccessResult("Review deleted successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized review deletion attempt for review {ReviewId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error deleting review: {ex.Message}"));
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(SupabaseService supabase, ILogger<ReviewController> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        // GET: api/review
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

        // GET: api/review/book/{bookId}
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

        // GET: api/review/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            try
            {
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

                return Ok(ApiResponse<object>.SuccessResult(reviews, $"Found {reviews?.Count ?? 0} reviews by user"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for user {UserId}", userId);
                return BadRequest(ApiResponse.ErrorResult($"Error getting user reviews: {ex.Message}"));
            }
        }

        // POST: api/review
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReviewDto reviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                // In a real app, get userId from JWT token
                var userId = Guid.NewGuid(); // Placeholder

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return BadRequest(ApiResponse.ErrorResult($"Error creating review: {ex.Message}"));
            }
        }

        // PUT: api/review/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ReviewDto reviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var updated = new Review
                {
                    Id = id,
                    BookId = reviewDto.BookId,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment
                };

                var response = await _supabase.Client.From<Review>().Update(updated);
                var review = response.Models?.FirstOrDefault();

                if (review == null)
                    return NotFound(ApiResponse.ErrorResult("Review not found or update failed"));

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error updating review: {ex.Message}"));
            }
        }

        // DELETE: api/review/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var review = new Review { Id = id };
                await _supabase.Client.From<Review>().Delete(review);

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
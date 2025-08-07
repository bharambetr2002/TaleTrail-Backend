using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : BaseController
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetReviewsForBook(System.Guid bookId)
        {
            var reviews = await _reviewService.GetReviewsForBookAsync(bookId);
            return Ok(ApiResponse<object>.SuccessResult(reviews));
        }

        [HttpGet("my-reviews")]
        [Authorize]
        public async Task<IActionResult> GetMyReviews()
        {
            var user = await GetCurrentUserAsync();
            var reviews = await _reviewService.GetReviewsByUserAsync(user.Id);
            return Ok(ApiResponse<object>.SuccessResult(reviews));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDto reviewDto)
        {
            var user = await GetCurrentUserAsync();
            var createdReview = await _reviewService.CreateReviewAsync(reviewDto, user.Id);
            return Ok(ApiResponse<object>.SuccessResult(createdReview));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(System.Guid id, [FromBody] ReviewDto reviewDto)
        {
            var user = await GetCurrentUserAsync();
            var updatedReview = await _reviewService.UpdateReviewAsync(id, reviewDto, user.Id);

            if (updatedReview == null) return Forbid();

            return Ok(ApiResponse<object>.SuccessResult(updatedReview));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(System.Guid id)
        {
            var user = await GetCurrentUserAsync();
            var success = await _reviewService.DeleteReviewAsync(id, user.Id);

            if (!success) return Forbid();

            return Ok(ApiResponse.SuccessResult("Review deleted."));
        }
    }
}
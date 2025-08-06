using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : BaseController
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(SupabaseService supabase, ILogger<FeedbackController> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        // GET: api/feedback
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _supabase.Client.From<Feedback>().Get();
                var feedbacks = response.Models?.Select(f => new
                {
                    f.Id,
                    f.UserId,
                    f.Message,
                    f.CreatedAt
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(feedbacks, $"Found {feedbacks?.Count ?? 0} feedback entries"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all feedback");
                return BadRequest(ApiResponse.ErrorResult($"Error getting feedback: {ex.Message}"));
            }
        }

        // GET: api/feedback/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            try
            {
                var response = await _supabase.Client
                    .From<Feedback>()
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Get();

                var feedbacks = response.Models?.Select(f => new
                {
                    f.Id,
                    f.UserId,
                    f.Message,
                    f.CreatedAt
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(feedbacks, $"Found {feedbacks?.Count ?? 0} feedback entries for user"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback for user {UserId}", userId);
                return BadRequest(ApiResponse.ErrorResult($"Error getting user feedback: {ex.Message}"));
            }
        }

        // GET: api/feedback/user/my-feedback
        [HttpGet("user/my-feedback")]
        public async Task<IActionResult> GetMyFeedback()
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _supabase.Client
                    .From<Feedback>()
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Get();

                var feedbacks = response.Models?.Select(f => new
                {
                    f.Id,
                    f.UserId,
                    f.Message,
                    f.CreatedAt
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(feedbacks, $"Found {feedbacks?.Count ?? 0} feedback entries"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to get user feedback");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user feedback");
                return BadRequest(ApiResponse.ErrorResult($"Error getting feedback: {ex.Message}"));
            }
        }

        // POST: api/feedback
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] FeedbackDto feedbackDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var userId = GetCurrentUserId();

                var feedback = new Feedback
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Message = feedbackDto.Message,
                    CreatedAt = DateTime.UtcNow
                };

                var response = await _supabase.Client.From<Feedback>().Insert(feedback);
                var createdFeedback = response.Models?.FirstOrDefault();

                if (createdFeedback == null)
                    return BadRequest(ApiResponse.ErrorResult("Failed to create feedback"));

                var result = new
                {
                    createdFeedback.Id,
                    createdFeedback.UserId,
                    createdFeedback.Message,
                    createdFeedback.CreatedAt
                };

                return Ok(ApiResponse<object>.SuccessResult(result, "Feedback created successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized feedback creation attempt");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating feedback");
                return BadRequest(ApiResponse.ErrorResult($"Error creating feedback: {ex.Message}"));
            }
        }

        // DELETE: api/feedback/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Get existing feedback and verify ownership
                var existingResponse = await _supabase.Client
                    .From<Feedback>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                var existingFeedback = existingResponse.Models?.FirstOrDefault();
                if (existingFeedback == null)
                    return NotFound(ApiResponse.ErrorResult("Feedback not found"));

                if (existingFeedback.UserId != userId)
                    return Forbid("You can only delete your own feedback");

                await _supabase.Client.From<Feedback>().Delete(existingFeedback);

                return Ok(ApiResponse.SuccessResult("Feedback deleted successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized feedback deletion attempt for feedback {FeedbackId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feedback {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error deleting feedback: {ex.Message}"));
            }
        }
    }
}
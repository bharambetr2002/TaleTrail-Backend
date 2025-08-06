using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : BaseController
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(SupabaseService supabase, ILogger<SubscriptionController> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _supabase.Client.From<Subscription>().Get();
                var subscriptions = response.Models?.Select(s => new
                {
                    s.Id,
                    s.UserId,
                    s.PlanName,
                    s.StartDate,
                    s.EndDate,
                    s.IsActive
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(subscriptions, $"Found {subscriptions?.Count ?? 0} subscriptions"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subscriptions");
                return BadRequest(ApiResponse.ErrorResult($"Error getting subscriptions: {ex.Message}"));
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            try
            {
                var response = await _supabase.Client
                    .From<Subscription>()
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Order("start_date", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                var subscriptions = response.Models?.Select(s => new
                {
                    s.Id,
                    s.UserId,
                    s.PlanName,
                    s.StartDate,
                    s.EndDate,
                    s.IsActive
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(subscriptions, $"Found {subscriptions?.Count ?? 0} subscriptions for user"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriptions for user {UserId}", userId);
                return BadRequest(ApiResponse.ErrorResult($"Error getting user subscriptions: {ex.Message}"));
            }
        }

        [HttpGet("user/my-subscriptions")]
        public async Task<IActionResult> GetMySubscriptions()
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _supabase.Client
                    .From<Subscription>()
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Order("start_date", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                var subscriptions = response.Models?.Select(s => new
                {
                    s.Id,
                    s.UserId,
                    s.PlanName,
                    s.StartDate,
                    s.EndDate,
                    s.IsActive
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(subscriptions, $"Found {subscriptions?.Count ?? 0} subscriptions"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to get user subscriptions");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user subscriptions");
                return BadRequest(ApiResponse.ErrorResult($"Error getting subscriptions: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubscriptionDto subscriptionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var userId = GetCurrentUserId();

                var subscription = new Subscription
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    PlanName = subscriptionDto.PlanName,
                    StartDate = subscriptionDto.StartDate,
                    EndDate = subscriptionDto.EndDate,
                    IsActive = true
                };

                var response = await _supabase.Client.From<Subscription>().Insert(subscription);
                var createdSubscription = response.Models?.FirstOrDefault();

                if (createdSubscription == null)
                    return BadRequest(ApiResponse.ErrorResult("Failed to create subscription"));

                var result = new
                {
                    createdSubscription.Id,
                    createdSubscription.UserId,
                    createdSubscription.PlanName,
                    createdSubscription.StartDate,
                    createdSubscription.EndDate,
                    createdSubscription.IsActive
                };

                return Ok(ApiResponse<object>.SuccessResult(result, "Subscription created successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized subscription creation attempt");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription");
                return BadRequest(ApiResponse.ErrorResult($"Error creating subscription: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SubscriptionDto subscriptionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var userId = GetCurrentUserId();

                // Get existing subscription and verify ownership
                var existingResponse = await _supabase.Client
                    .From<Subscription>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                var existingSubscription = existingResponse.Models?.FirstOrDefault();
                if (existingSubscription == null)
                    return NotFound(ApiResponse.ErrorResult("Subscription not found"));

                if (existingSubscription.UserId != userId)
                    return Forbid("You can only update your own subscriptions");

                existingSubscription.PlanName = subscriptionDto.PlanName;
                existingSubscription.StartDate = subscriptionDto.StartDate;
                existingSubscription.EndDate = subscriptionDto.EndDate;

                var response = await _supabase.Client.From<Subscription>().Update(existingSubscription);
                var updatedSubscription = response.Models?.FirstOrDefault();

                if (updatedSubscription == null)
                    return BadRequest(ApiResponse.ErrorResult("Failed to update subscription"));

                var result = new
                {
                    updatedSubscription.Id,
                    updatedSubscription.UserId,
                    updatedSubscription.PlanName,
                    updatedSubscription.StartDate,
                    updatedSubscription.EndDate,
                    updatedSubscription.IsActive
                };

                return Ok(ApiResponse<object>.SuccessResult(result, "Subscription updated successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized subscription update attempt for subscription {SubscriptionId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error updating subscription: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Get existing subscription and verify ownership
                var existingResponse = await _supabase.Client
                    .From<Subscription>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                var existingSubscription = existingResponse.Models?.FirstOrDefault();
                if (existingSubscription == null)
                    return NotFound(ApiResponse.ErrorResult("Subscription not found"));

                if (existingSubscription.UserId != userId)
                    return Forbid("You can only delete your own subscriptions");

                await _supabase.Client.From<Subscription>().Delete(existingSubscription);

                return Ok(ApiResponse.SuccessResult("Subscription deleted successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized subscription deletion attempt for subscription {SubscriptionId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error deleting subscription: {ex.Message}"));
            }
        }
    }
}
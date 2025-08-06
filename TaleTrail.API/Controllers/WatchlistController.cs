using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WatchlistController : BaseController
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<WatchlistController> _logger;

        public WatchlistController(SupabaseService supabase, ILogger<WatchlistController> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserWatchlist(Guid userId)
        {
            try
            {
                var response = await _supabase.Client
                    .From<Watchlist>()
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Order("added_at", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                var watchlists = response.Models?.Select(w => new
                {
                    w.Id,
                    w.UserId,
                    w.BookId,
                    w.Status,
                    w.AddedAt
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(watchlists, $"Found {watchlists?.Count ?? 0} watchlist items"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting watchlist for user {UserId}", userId);
                return BadRequest(ApiResponse.ErrorResult($"Error getting watchlist: {ex.Message}"));
            }
        }

        [HttpGet("user/my-watchlist")]
        public async Task<IActionResult> GetMyWatchlist()
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _supabase.Client
                    .From<Watchlist>()
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Order("added_at", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                var watchlists = response.Models?.Select(w => new
                {
                    w.Id,
                    w.UserId,
                    w.BookId,
                    w.Status,
                    w.AddedAt
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(watchlists, $"Found {watchlists?.Count ?? 0} watchlist items"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to get user watchlist");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user watchlist");
                return BadRequest(ApiResponse.ErrorResult($"Error getting watchlist: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToWatchlist([FromBody] WatchlistDto watchlistDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var userId = GetCurrentUserId();

                // Check if already exists
                var existingResponse = await _supabase.Client
                    .From<Watchlist>()
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Filter("book_id", Supabase.Postgrest.Constants.Operator.Equals, watchlistDto.BookId.ToString())
                    .Get();

                if (existingResponse.Models?.Any() == true)
                    return BadRequest(ApiResponse.ErrorResult("Book is already in your watchlist"));

                var watchlist = new Watchlist
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    BookId = watchlistDto.BookId,
                    Status = watchlistDto.Status,
                    AddedAt = DateTime.UtcNow
                };

                var response = await _supabase.Client.From<Watchlist>().Insert(watchlist);
                var createdWatchlist = response.Models?.FirstOrDefault();

                if (createdWatchlist == null)
                    return BadRequest(ApiResponse.ErrorResult("Failed to add book to watchlist"));

                var result = new
                {
                    createdWatchlist.Id,
                    createdWatchlist.UserId,
                    createdWatchlist.BookId,
                    createdWatchlist.Status,
                    createdWatchlist.AddedAt
                };

                return Ok(ApiResponse<object>.SuccessResult(result, "Book added to watchlist successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to add to watchlist");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding book to watchlist");
                return BadRequest(ApiResponse.ErrorResult($"Error adding to watchlist: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] WatchlistDto watchlistDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var userId = GetCurrentUserId();

                // Get existing watchlist item and verify ownership
                var existingResponse = await _supabase.Client
                    .From<Watchlist>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                var existingWatchlist = existingResponse.Models?.FirstOrDefault();
                if (existingWatchlist == null)
                    return NotFound(ApiResponse.ErrorResult("Watchlist item not found"));

                if (existingWatchlist.UserId != userId)
                    return Forbid("You can only update your own watchlist items");

                existingWatchlist.Status = watchlistDto.Status;
                existingWatchlist.BookId = watchlistDto.BookId;

                var response = await _supabase.Client.From<Watchlist>().Update(existingWatchlist);
                var updatedWatchlist = response.Models?.FirstOrDefault();

                if (updatedWatchlist == null)
                    return BadRequest(ApiResponse.ErrorResult("Failed to update watchlist item"));

                var result = new
                {
                    updatedWatchlist.Id,
                    updatedWatchlist.UserId,
                    updatedWatchlist.BookId,
                    updatedWatchlist.Status,
                    updatedWatchlist.AddedAt
                };

                return Ok(ApiResponse<object>.SuccessResult(result, "Watchlist updated successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to update watchlist item {WatchlistId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating watchlist item {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error updating watchlist: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromWatchlist(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Get existing watchlist item and verify ownership
                var existingResponse = await _supabase.Client
                    .From<Watchlist>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                var existingWatchlist = existingResponse.Models?.FirstOrDefault();
                if (existingWatchlist == null)
                    return NotFound(ApiResponse.ErrorResult("Watchlist item not found"));

                if (existingWatchlist.UserId != userId)
                    return Forbid("You can only delete your own watchlist items");

                await _supabase.Client.From<Watchlist>().Delete(existingWatchlist);

                return Ok(ApiResponse.SuccessResult("Book removed from watchlist successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to remove from watchlist {WatchlistId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from watchlist {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error removing from watchlist: {ex.Message}"));
            }
        }
    }
}
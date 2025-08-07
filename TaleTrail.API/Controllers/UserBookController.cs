using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaleTrail.API.Exceptions;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/user-book")]
    [Authorize] // All actions in this controller require an authenticated user.
    public class UserBookController : BaseController
    {
        private readonly UserBookService _userBookService;
        private readonly ILogger<UserBookController> _logger;

        public UserBookController(UserBookService userBookService, ILogger<UserBookController> logger)
        {
            _userBookService = userBookService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the reading list for the currently authenticated user.
        /// </summary>
        [HttpGet("my-list")]
        public async Task<IActionResult> GetMyReadingList()
        {
            try
            {
                // Validate user exists in database before proceeding
                var user = await GetCurrentUserAsync();
                LogUserAction("GetMyReadingList");

                var userBooks = await _userBookService.GetUserReadingListAsync(user.Id);
                return Ok(ApiResponse<object>.SuccessResult(userBooks, $"Found {userBooks.Count} books in your reading list"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("🚫 Unauthorized access to reading list: {Error}", ex.Message);
                return Unauthorized(ApiResponse.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting reading list for user: {UserInfo}", GetUserInfoForLogging());
                return BadRequest(ApiResponse.ErrorResult("Could not retrieve your reading list."));
            }
        }

        /// <summary>
        /// Adds a book to the user's reading list or updates its status.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateUserBook([FromBody] UserBookDTO userBookDto)
        {
            try
            {
                // Validate user exists in database before proceeding
                var user = await GetCurrentUserAsync();
                LogUserAction("AddOrUpdateUserBook", new { BookId = userBookDto.BookId, Status = userBookDto.Status });

                var result = await _userBookService.AddOrUpdateUserBookAsync(user.Id, userBookDto);

                return Ok(ApiResponse<object>.SuccessResult(result, "Book status updated successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("🚫 Unauthorized access to update book status: {Error}", ex.Message);
                return Unauthorized(ApiResponse.ErrorResult(ex.Message));
            }
            catch (AppException ex) // Catches business rule violations (e.g., too many in-progress books)
            {
                _logger.LogInformation("📋 Business rule violation for user {UserInfo}: {Error}", GetUserInfoForLogging(), ex.Message);
                return BadRequest(ApiResponse.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error adding or updating user book for {UserInfo}", GetUserInfoForLogging());
                return BadRequest(ApiResponse.ErrorResult("An error occurred while updating your list."));
            }
        }

        /// <summary>
        /// Removes a book from the user's reading list.
        /// </summary>
        /// <param name="bookId">The ID of the book to remove from the list.</param>
        [HttpDelete("{bookId}")]
        public async Task<IActionResult> RemoveUserBook(Guid bookId)
        {
            try
            {
                // Validate user exists in database before proceeding
                var user = await GetCurrentUserAsync();
                LogUserAction("RemoveUserBook", new { BookId = bookId });

                var success = await _userBookService.RemoveUserBookAsync(user.Id, bookId);
                if (!success)
                {
                    return NotFound(ApiResponse.ErrorResult("The specified book was not found on your list."));
                }
                return Ok(ApiResponse.SuccessResult("Book removed from your list successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("🚫 Unauthorized access to remove book: {Error}", ex.Message);
                return Unauthorized(ApiResponse.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error removing book {BookId} from user's list: {UserInfo}", bookId, GetUserInfoForLogging());
                return BadRequest(ApiResponse.ErrorResult("An error occurred while removing the book."));
            }
        }

        /// <summary>
        /// Gets reading statistics for the current user
        /// </summary>
        [HttpGet("my-stats")]
        public async Task<IActionResult> GetMyReadingStats()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var userBooks = await _userBookService.GetUserReadingListAsync(user.Id);

                var stats = new
                {
                    totalBooks = userBooks.Count,
                    wannaRead = userBooks.Count(ub => ub.Status == "wanna_read"),
                    inProgress = userBooks.Count(ub => ub.Status == "in_progress"),
                    completed = userBooks.Count(ub => ub.Status == "completed"),
                    dropped = userBooks.Count(ub => ub.Status == "dropped"),
                    canAddMoreInProgress = userBooks.Count(ub => ub.Status == "in_progress") < 3
                };

                return Ok(ApiResponse<object>.SuccessResult(stats, "Reading statistics retrieved successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting reading stats for user: {UserInfo}", GetUserInfoForLogging());
                return BadRequest(ApiResponse.ErrorResult("Could not retrieve reading statistics."));
            }
        }

        /// <summary>
        /// Updates multiple book statuses at once (batch operation)
        /// </summary>
        [HttpPatch("batch-update")]
        public async Task<IActionResult> BatchUpdateUserBooks([FromBody] BatchUpdateUserBooksDTO request)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                LogUserAction("BatchUpdateUserBooks", new { Count = request.Updates.Count });

                var results = new List<object>();
                var errors = new List<string>();

                foreach (var update in request.Updates)
                {
                    try
                    {
                        var result = await _userBookService.AddOrUpdateUserBookAsync(user.Id, update);
                        results.Add(new { bookId = update.BookId, status = "success", data = result });
                    }
                    catch (AppException ex)
                    {
                        errors.Add($"Book {update.BookId}: {ex.Message}");
                        results.Add(new { bookId = update.BookId, status = "error", error = ex.Message });
                    }
                }

                var response = new
                {
                    processedCount = request.Updates.Count,
                    successCount = results.Count(r => ((dynamic)r).status == "success"),
                    errorCount = errors.Count,
                    results = results,
                    errors = errors
                };

                if (errors.Any())
                {
                    return BadRequest(ApiResponse<object>.SuccessResult(response, "Batch update completed with some errors."));
                }

                return Ok(ApiResponse<object>.SuccessResult(response, "Batch update completed successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in batch update for user: {UserInfo}", GetUserInfoForLogging());
                return BadRequest(ApiResponse.ErrorResult("An error occurred during batch update."));
            }
        }
    }

    public class BatchUpdateUserBooksDTO
    {
        public List<UserBookDTO> Updates { get; set; } = new List<UserBookDTO>();
    }
}
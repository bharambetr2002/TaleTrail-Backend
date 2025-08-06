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
                var userId = GetCurrentUserId();
                var userBooks = await _userBookService.GetUserReadingListAsync(userId);
                return Ok(ApiResponse<object>.SuccessResult(userBooks));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reading list for user");
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
                var userId = GetCurrentUserId();
                var result = await _userBookService.AddOrUpdateUserBookAsync(userId, userBookDto);
                return Ok(ApiResponse<object>.SuccessResult(result, "Book status updated successfully."));
            }
            catch (AppException ex) // Catches business rule violations (e.g., too many in-progress books)
            {
                return BadRequest(ApiResponse.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding or updating user book");
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
                var userId = GetCurrentUserId();
                var success = await _userBookService.RemoveUserBookAsync(userId, bookId);
                if (!success)
                {
                    return NotFound(ApiResponse.ErrorResult("The specified book was not found on your list."));
                }
                return Ok(ApiResponse.SuccessResult("Book removed from your list successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing book {BookId} from user's list", bookId);
                return BadRequest(ApiResponse.ErrorResult("An error occurred while removing the book."));
            }
        }
    }
}

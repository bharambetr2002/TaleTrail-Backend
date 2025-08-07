using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;
using System;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/user-book")]
    [Authorize] // All actions still require a logged-in user
    public class UserBookController : BaseController
    {
        private readonly UserBookService _userBookService;

        public UserBookController(UserBookService userBookService)
        {
            _userBookService = userBookService;
        }

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
                return BadRequest(ApiResponse.ErrorResult(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateUserBook([FromBody] UserBookDTO userBookDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _userBookService.AddOrUpdateUserBookAsync(userId, userBookDto);
                return Ok(ApiResponse<object>.SuccessResult(result, "Book status updated."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.ErrorResult(ex.Message));
            }
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> RemoveUserBook(Guid bookId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _userBookService.RemoveUserBookAsync(userId, bookId);
                if (!success)
                {
                    return NotFound(ApiResponse.ErrorResult("Book not found on your list."));
                }
                return Ok(ApiResponse.SuccessResult("Book removed from your list."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.ErrorResult(ex.Message));
            }
        }
    }
}
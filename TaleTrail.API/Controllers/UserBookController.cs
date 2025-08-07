using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/user-book")]
    [Authorize]
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
            var user = await GetCurrentUserAsync();
            var userBooks = await _userBookService.GetUserReadingListAsync(user.Id);
            return Ok(ApiResponse<object>.SuccessResult(userBooks));
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateUserBook([FromBody] UserBookDTO userBookDto)
        {
            var user = await GetCurrentUserAsync();
            var result = await _userBookService.AddOrUpdateUserBookAsync(user.Id, userBookDto);
            return Ok(ApiResponse<object>.SuccessResult(result, "Book status updated."));
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> RemoveUserBook(System.Guid bookId)
        {
            var user = await GetCurrentUserAsync();
            var success = await _userBookService.RemoveUserBookAsync(user.Id, bookId);
            if (!success)
            {
                return NotFound(ApiResponse.ErrorResult("Book not found on your list."));
            }
            return Ok(ApiResponse.SuccessResult("Book removed from your list."));
        }
    }
}
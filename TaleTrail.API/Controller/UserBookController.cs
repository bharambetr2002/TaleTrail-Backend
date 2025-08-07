using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/user-book")]
[Authorize]
public class UserBookController : BaseController
{
    private readonly UserBookService _userBookService;

    public UserBookController(UserService userService, UserBookService userBookService)
        : base(userService)
    {
        _userBookService = userBookService;
    }

    [HttpGet("my-list")]
    public async Task<IActionResult> GetMyBooks()
    {
        var userId = GetCurrentUserId();
        var books = await _userBookService.GetUserBooksAsync(userId);
        return Ok(ApiResponse<List<UserBook>>.SuccessResponse("Fetched user books", books));
    }

    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] AddUserBookRequest request)
    {
        var userId = GetCurrentUserId();
        var userBook = await _userBookService.AddBookToUserListAsync(userId, request);
        return Ok(ApiResponse<UserBook>.SuccessResponse("Book added to list", userBook));
    }

    [HttpPut("{bookId}")]
    public async Task<IActionResult> UpdateBook(Guid bookId, [FromBody] UpdateUserBookRequest request)
    {
        var userId = GetCurrentUserId();
        var userBook = await _userBookService.UpdateUserBookAsync(userId, bookId, request);
        return Ok(ApiResponse<UserBook>.SuccessResponse("Book updated", userBook));
    }

    [HttpDelete("{bookId}")]
    public async Task<IActionResult> RemoveBook(Guid bookId)
    {
        var userId = GetCurrentUserId();
        await _userBookService.RemoveBookFromUserListAsync(userId, bookId);
        return Ok(ApiResponse<object>.SuccessResponse("Book removed from list", null));
    }
}
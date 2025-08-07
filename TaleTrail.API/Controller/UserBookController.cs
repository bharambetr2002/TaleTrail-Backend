using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserBookController : BaseController
{
    private readonly UserBookService _userBookService;

    public UserBookController(UserService userService, UserBookService userBookService, ILogger<UserBookController> logger)
        : base(userService, logger)
    {
        _userBookService = userBookService;
    }

    [HttpGet("my-books")]
    public async Task<IActionResult> GetMyBooks()
    {
        try
        {
            var userId = GetCurrentUserId();
            var books = await _userBookService.GetUserBooksAsync(userId);
            return Ok(ApiResponse<List<UserBookResponseDTO>>.SuccessResponse($"Retrieved {books.Count} books from your library", books));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<UserBookResponseDTO>>.ErrorResponse($"Failed to retrieve your books: {ex.Message}"));
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] AddUserBookRequestDTO request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userBook = await _userBookService.AddBookToUserListAsync(userId, request);
            return Ok(ApiResponse<UserBookResponseDTO>.SuccessResponse("Book added to your library", userBook));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserBookResponseDTO>.ErrorResponse($"Failed to add book: {ex.Message}"));
        }
    }

    [HttpPut("{bookId}")]
    public async Task<IActionResult> UpdateBook(Guid bookId, [FromBody] UpdateUserBookRequestDTO request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userBook = await _userBookService.UpdateUserBookAsync(userId, bookId, request);
            return Ok(ApiResponse<UserBookResponseDTO>.SuccessResponse("Book status updated successfully", userBook));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserBookResponseDTO>.ErrorResponse($"Failed to update book: {ex.Message}"));
        }
    }

    [HttpDelete("{bookId}")]
    public async Task<IActionResult> RemoveBook(Guid bookId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _userBookService.RemoveBookFromUserListAsync(userId, bookId);
            return Ok(ApiResponse<string?>.SuccessResponse("Book removed from your library", null));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string?>.ErrorResponse($"Failed to remove book: {ex.Message}"));
        }
    }
}
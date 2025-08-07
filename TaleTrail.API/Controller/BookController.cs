using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly BookService _bookService;

    public BookController(BookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        try
        {
            var books = await _bookService.GetAllBooksAsync(search);
            var message = string.IsNullOrEmpty(search)
                ? "Books retrieved successfully"
                : $"Found {books.Count} books matching '{search}'";

            return Ok(ApiResponse<List<BookResponseDTO>>.SuccessResponse(message, books));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<BookResponseDTO>>.ErrorResponse($"Failed to retrieve books: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound(ApiResponse<BookResponseDTO>.ErrorResponse("Book not found"));

            return Ok(ApiResponse<BookResponseDTO>.SuccessResponse("Book retrieved successfully", book));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<BookResponseDTO>.ErrorResponse($"Failed to retrieve book: {ex.Message}"));
        }
    }

    [HttpGet("by-author/{authorId}")]
    public async Task<IActionResult> GetByAuthor(Guid authorId)
    {
        try
        {
            var books = await _bookService.GetBooksByAuthorAsync(authorId);
            return Ok(ApiResponse<List<BookResponseDTO>>.SuccessResponse($"Found {books.Count} books by this author", books));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<BookResponseDTO>>.ErrorResponse($"Failed to retrieve author's books: {ex.Message}"));
        }
    }
}
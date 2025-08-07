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
        var books = await _bookService.GetAllBooksAsync(search);
        return Ok(ApiResponse<List<BookResponseDTO>>.SuccessResponse("Books retrieved successfully", books));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound(ApiResponse<BookResponseDTO>.ErrorResponse("Book not found"));

        return Ok(ApiResponse<BookResponseDTO>.SuccessResponse("Book retrieved successfully", book));
    }

    [HttpGet("by-author/{authorId}")]
    public async Task<IActionResult> GetByAuthor(Guid authorId)
    {
        var books = await _bookService.GetBooksByAuthorAsync(authorId);
        return Ok(ApiResponse<List<BookResponseDTO>>.SuccessResponse("Author's books retrieved successfully", books));
    }
}
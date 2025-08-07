using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    // ✅ GET /api/book?search=keyword
    [HttpGet]
    public async Task<IActionResult> GetAllBooks([FromQuery] string? search)
    {
        var books = await _bookService.GetAllBooksAsync(search);
        return Ok(new { success = true, message = "Success", data = books });
    }

    // ✅ GET /api/book/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(Guid id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound(new { success = false, message = "Book not found" });

        return Ok(new { success = true, message = "Success", data = book });
    }

    // ✅ GET /api/book/by-author/{authorId}
    [HttpGet("by-author/{authorId}")]
    public async Task<IActionResult> GetBooksByAuthor(Guid authorId)
    {
        var books = await _bookService.GetBooksByAuthorAsync(authorId);
        return Ok(new { success = true, message = "Success", data = books });
    }
}
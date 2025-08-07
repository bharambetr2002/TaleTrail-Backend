// File: Controllers/BookController.cs
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Model;
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
    public async Task<IActionResult> GetAll()
    {
        var books = await _bookService.GetAllBooksAsync();
        return Ok(new { success = true, message = "Fetched all books", data = books });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound(new { success = false, message = "Book not found" });

        return Ok(new { success = true, message = "Fetched book", data = book });
    }
}
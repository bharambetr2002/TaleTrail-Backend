using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Models;

namespace TaleTrail.API.Controllers
{
    [ApController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly BookService _bookService;
        private readonly ILogger<BookController> _logger;

        public BookController(BookService bookService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            return Ok(book);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateBook(Guid userId, [FromBody] BookDTO dto)
        {
            var book = await _bookService.CreateBookAsync(dto, userId);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookDTO dto)
        {
            var updated = await _bookService.UpdateBookAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }
    }
}
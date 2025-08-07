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
    [Route("api/[controller]")]
    public class BookController : BaseController
    {
        private readonly BookService _bookService;
        // This is where the ILogger was, but we've removed it for simplicity.
        // It's good practice to add it back later for production code.
        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/book
        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] string? search = null)
        {
            var books = await _bookService.GetAllBooksAsync(search);
            return Ok(ApiResponse<object>.SuccessResult(books));
        }

        // GET: api/book/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound(ApiResponse.ErrorResult("Book not found"));
            }
            return Ok(ApiResponse<object>.SuccessResult(book));
        }

        // POST: api/book
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateBook([FromBody] BookDto bookDto)
        {
            var book = await _bookService.CreateBookAsync(bookDto);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, ApiResponse<object>.SuccessResult(book, "Book created."));
        }

        // PUT: api/book/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookDto bookDto)
        {
            var book = await _bookService.UpdateBookAsync(id, bookDto);
            if (book == null)
            {
                return NotFound(ApiResponse.ErrorResult("Book not found"));
            }
            return Ok(ApiResponse<object>.SuccessResult(book, "Book updated."));
        }

        // DELETE: api/book/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var success = await _bookService.DeleteBookAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse.ErrorResult("Book not found"));
            }
            return Ok(ApiResponse.SuccessResult("Book deleted."));
        }
    }
}
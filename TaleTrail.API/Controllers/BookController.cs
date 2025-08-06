using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Add this for authorization
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
        private readonly ILogger<BookController> _logger;

        public BookController(BookService bookService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        // GET: api/book - This is public
        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] string? search = null)
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync(search);
                return Ok(ApiResponse<object>.SuccessResult(books, $"Found {books.Count} books"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all books");
                return BadRequest(ApiResponse.ErrorResult($"Error getting books: {ex.Message}"));
            }
        }

        // GET: api/book/{id} - This is public
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound(ApiResponse.ErrorResult("Book not found"));
                }
                return Ok(ApiResponse<object>.SuccessResult(book));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting book {BookId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error getting book: {ex.Message}"));
            }
        }

        // POST: api/book - ADMIN ONLY
        [HttpPost]
        [Authorize(Roles = "admin")] // IMPORTANT: Secures the endpoint
        public async Task<IActionResult> CreateBook([FromBody] BookDto bookDto)
        {
            try
            {
                var book = await _bookService.CreateBookAsync(bookDto);
                return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, ApiResponse<object>.SuccessResult(book, "Book created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return BadRequest(ApiResponse.ErrorResult($"Failed to create book: {ex.Message}"));
            }
        }

        // PUT: api/book/{id} - ADMIN ONLY
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")] // IMPORTANT: Secures the endpoint
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookDto bookDto)
        {
            try
            {
                var book = await _bookService.UpdateBookAsync(id, bookDto);
                if (book == null)
                {
                    return NotFound(ApiResponse.ErrorResult("Book not found"));
                }
                return Ok(ApiResponse<object>.SuccessResult(book, "Book updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book {BookId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Failed to update book: {ex.Message}"));
            }
        }

        // DELETE: api/book/{id} - ADMIN ONLY
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")] // IMPORTANT: Secures the endpoint
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            try
            {
                var success = await _bookService.DeleteBookAsync(id);
                if (!success)
                {
                    return NotFound(ApiResponse.ErrorResult("Book not found"));
                }
                return Ok(ApiResponse.SuccessResult("Book deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book {BookId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Failed to delete book: {ex.Message}"));
            }
        }

        // The "user/my-books" endpoint has been removed.
        // This logic belongs in the UserBookController.
    }
}
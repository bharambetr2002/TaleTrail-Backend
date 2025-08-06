using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;

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

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                return Ok(ApiResponse<object>.SuccessResult(books, $"Found {books.Count} books"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all books");
                return BadRequest(ApiResponse.ErrorResult($"Error getting books: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                return Ok(ApiResponse<object>.SuccessResult(book));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting book {BookId}", id);
                return NotFound(ApiResponse.ErrorResult($"Book not found: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookDto bookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                // Get user ID from JWT token via middleware
                var userId = GetCurrentUserId();

                var book = await _bookService.CreateBookAsync(bookDto, userId);
                return Ok(ApiResponse<object>.SuccessResult(book, "Book created successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized book creation attempt");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return BadRequest(ApiResponse.ErrorResult($"Failed to create book: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookDto bookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                // Get user ID from JWT token via middleware
                var userId = GetCurrentUserId();

                var book = await _bookService.UpdateBookAsync(id, bookDto, userId);
                return Ok(ApiResponse<object>.SuccessResult(book, "Book updated successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized book update attempt for book {BookId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book {BookId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Failed to update book: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            try
            {
                // Get user ID from JWT token via middleware
                var userId = GetCurrentUserId();

                await _bookService.DeleteBookAsync(id, userId);
                return Ok(ApiResponse.SuccessResult("Book deleted successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized book deletion attempt for book {BookId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book {BookId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Failed to delete book: {ex.Message}"));
            }
        }

        [HttpGet("user/my-books")]
        public async Task<IActionResult> GetMyBooks()
        {
            try
            {
                var userId = GetCurrentUserId();
                var books = await _bookService.GetUserBooksAsync(userId);
                return Ok(ApiResponse<object>.SuccessResult(books, $"Found {books.Count} books"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to get user books");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user books");
                return BadRequest(ApiResponse.ErrorResult($"Error getting books: {ex.Message}"));
            }
        }
    }
}
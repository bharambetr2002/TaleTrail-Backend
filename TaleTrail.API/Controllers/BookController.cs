using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
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
        public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var book = await _bookService.CreateBookAsync(request.BookDto, request.UserId);
                return Ok(ApiResponse<object>.SuccessResult(book, "Book created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book for user {UserId}", request.UserId);
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

                var book = await _bookService.UpdateBookAsync(id, bookDto);
                return Ok(ApiResponse<object>.SuccessResult(book, "Book updated successfully"));
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
                await _bookService.DeleteBookAsync(id);
                return Ok(ApiResponse.SuccessResult("Book deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book {BookId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Failed to delete book: {ex.Message}"));
            }
        }
    }

    // Request model to handle both userId and bookDto
    public class CreateBookRequest
    {
        public Guid UserId { get; set; }
        public BookDto BookDto { get; set; } = new BookDto();
    }
}
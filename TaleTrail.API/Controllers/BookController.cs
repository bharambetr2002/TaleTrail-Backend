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

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(ApiResponse<object>.SuccessResult(books));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            return Ok(ApiResponse<object>.SuccessResult(book));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookDto bookDto)
        {
            // In a real app, you'd get userId from JWT token
            var userId = Guid.NewGuid(); // Placeholder
            var book = await _bookService.CreateBookAsync(bookDto, userId);
            return Ok(ApiResponse<object>.SuccessResult(book, "Book created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookDto bookDto)
        {
            var book = await _bookService.UpdateBookAsync(id, bookDto);
            return Ok(ApiResponse<object>.SuccessResult(book, "Book updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await _bookService.DeleteBookAsync(id);
            return Ok(ApiResponse.SuccessResult("Book deleted successfully"));
        }
    }
}
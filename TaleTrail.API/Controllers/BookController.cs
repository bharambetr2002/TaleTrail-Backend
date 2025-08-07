using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;
using System;
using System.Threading.Tasks;
using System.Linq; // Add this for .Select()
using TaleTrail.API.DTOs; // Add this

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : BaseController
    {
        private readonly BookService _bookService;

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] string? search = null)
        {
            var books = await _bookService.GetAllBooksAsync(search);

            // --- THE FIX ---
            // Convert the list of Book models to a list of clean BookResponseDTOs
            var bookDtos = books.Select(b => new BookResponseDTO
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                PublicationYear = b.PublicationYear
            }).ToList();
            // -----------------

            return Ok(ApiResponse<object>.SuccessResult(bookDtos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound(ApiResponse.ErrorResult("Book not found"));
            }

            var bookDto = new BookResponseDTO
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                PublicationYear = book.PublicationYear
            };

            return Ok(ApiResponse<object>.SuccessResult(bookDto));
        }
    }
}
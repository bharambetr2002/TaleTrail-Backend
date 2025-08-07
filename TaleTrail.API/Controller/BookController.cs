using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model; // Fixed namespace
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

/// <summary>
/// Controller for managing books in the catalog
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Books")]
[Produces("application/json")]
public class BookController : ControllerBase
{
    private readonly BookService _bookService;
    private readonly ILogger<BookController> _logger;

    public BookController(BookService bookService, ILogger<BookController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    /// <summary>
    /// Get all books with optional search functionality
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<Book>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0)
    {
        try
        {
            // Validate parameters
            if (limit <= 0 || limit > 100)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Limit must be between 1 and 100"));
            }

            if (offset < 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Offset must be 0 or greater"));
            }

            _logger.LogInformation("Fetching books with search: {Search}, limit: {Limit}, offset: {Offset}",
                search ?? "none", limit, offset);

            List<Book> books;

            if (!string.IsNullOrWhiteSpace(search))
            {
                books = await _bookService.SearchBooksAsync(search);
            }
            else
            {
                books = await _bookService.GetAllBooksAsync();
            }

            // Apply pagination
            var paginatedBooks = books.Skip(offset).Take(limit).ToList();

            var message = string.IsNullOrWhiteSpace(search)
                ? $"Fetched {paginatedBooks.Count} books"
                : $"Found {paginatedBooks.Count} books matching '{search}'";

            return Ok(ApiResponse<List<Book>>.SuccessResponse(message, paginatedBooks));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching books");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while fetching books"));
        }
    }

    /// <summary>
    /// Get a specific book by its ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<Book>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid book ID format"));
            }

            _logger.LogInformation("Fetching book with ID: {BookId}", id);

            var book = await _bookService.GetBookByIdAsync(id);

            if (book == null)
            {
                _logger.LogWarning("Book not found with ID: {BookId}", id);
                return NotFound(ApiResponse<object>.ErrorResponse("Book not found"));
            }

            return Ok(ApiResponse<Book>.SuccessResponse("Book retrieved successfully", book));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching book with ID: {BookId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while fetching the book"));
        }
    }

    /// <summary>
    /// Search books by various criteria
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<List<Book>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> SearchBooks(
        [FromQuery] string? title = null,
        [FromQuery] string? author = null,
        [FromQuery] string? language = null,
        [FromQuery] int? yearFrom = null,
        [FromQuery] int? yearTo = null)
    {
        try
        {
            // Validate year parameters
            if (yearFrom.HasValue && (yearFrom.Value < 0 || yearFrom.Value > DateTime.Now.Year))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid 'yearFrom' parameter"));
            }

            if (yearTo.HasValue && (yearTo.Value < 0 || yearTo.Value > DateTime.Now.Year))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid 'yearTo' parameter"));
            }

            if (yearFrom.HasValue && yearTo.HasValue && yearFrom > yearTo)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("'yearFrom' cannot be greater than 'yearTo'"));
            }

            _logger.LogInformation("Advanced book search - Title: {Title}, Author: {Author}, Language: {Language}, Years: {YearFrom}-{YearTo}",
                title ?? "any", author ?? "any", language ?? "any", yearFrom, yearTo);

            var books = await _bookService.SearchBooksAdvancedAsync(title, author, language, yearFrom, yearTo);

            var searchCriteria = new List<string>();
            if (!string.IsNullOrEmpty(title)) searchCriteria.Add($"title: {title}");
            if (!string.IsNullOrEmpty(author)) searchCriteria.Add($"author: {author}");
            if (!string.IsNullOrEmpty(language)) searchCriteria.Add($"language: {language}");
            if (yearFrom.HasValue) searchCriteria.Add($"from year: {yearFrom}");
            if (yearTo.HasValue) searchCriteria.Add($"to year: {yearTo}");

            var message = searchCriteria.Any()
                ? $"Found {books.Count} books matching criteria: {string.Join(", ", searchCriteria)}"
                : $"Found {books.Count} books";

            return Ok(ApiResponse<List<Book>>.SuccessResponse(message, books));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during advanced book search");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred during the search"));
        }
    }

    /// <summary>
    /// Get books by a specific author
    /// </summary>
    [HttpGet("by-author/{authorId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<Book>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetBooksByAuthor(Guid authorId)
    {
        try
        {
            if (authorId == Guid.Empty)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid author ID format"));
            }

            _logger.LogInformation("Fetching books by author ID: {AuthorId}", authorId);

            var books = await _bookService.GetBooksByAuthorAsync(authorId);

            if (!books.Any())
            {
                return NotFound(ApiResponse<object>.ErrorResponse("No books found for this author"));
            }

            return Ok(ApiResponse<List<Book>>.SuccessResponse($"Found {books.Count} books by this author", books));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching books by author ID: {AuthorId}", authorId);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while fetching books by author"));
        }
    }
}
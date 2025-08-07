using TaleTrail.API.DAO;
using TaleTrail.API.Model.Entities;

namespace TaleTrail.API.Services;

/// <summary>
/// Service for handling book-related business logic
/// </summary>
public class BookService
{
    private readonly BookDao _bookDao;
    private readonly ILogger<BookService> _logger;

    public BookService(BookDao bookDao, ILogger<BookService> logger)
    {
        _bookDao = bookDao;
        _logger = logger;
    }

    /// <summary>
    /// Get all books from the catalog
    /// </summary>
    /// <returns>List of all books</returns>
    public async Task<List<Book>> GetAllBooksAsync()
    {
        try
        {
            _logger.LogDebug("Retrieving all books from catalog");
            return await _bookDao.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all books");
            throw new InvalidOperationException("Failed to retrieve books", ex);
        }
    }

    /// <summary>
    /// Get a specific book by its ID
    /// </summary>
    /// <param name="id">Book ID</param>
    /// <returns>Book if found, null otherwise</returns>
    public async Task<Book?> GetBookByIdAsync(Gui
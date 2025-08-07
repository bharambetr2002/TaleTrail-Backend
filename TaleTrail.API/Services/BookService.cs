using TaleTrail.API.DAO;
using TaleTrail.API.Model;

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
    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        try
        {
            _logger.LogDebug("Retrieving book with ID: {BookId}", id);
            return await _bookDao.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book with ID: {BookId}", id);
            throw new InvalidOperationException($"Failed to retrieve book with ID: {id}", ex);
        }
    }

    /// <summary>
    /// Search books by title
    /// </summary>
    /// <param name="searchTerm">Search term to match against book titles</param>
    /// <returns>List of books matching the search term</returns>
    public async Task<List<Book>> SearchBooksAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Book>();

            _logger.LogDebug("Searching books with term: {SearchTerm}", searchTerm);
            return await _bookDao.SearchByTitleAsync(searchTerm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching books with term: {SearchTerm}", searchTerm);
            throw new InvalidOperationException($"Failed to search books with term: {searchTerm}", ex);
        }
    }

    /// <summary>
    /// Advanced search for books by multiple criteria
    /// </summary>
    public async Task<List<Book>> SearchBooksAdvancedAsync(string? title, string? author, string? language, int? yearFrom, int? yearTo)
    {
        try
        {
            _logger.LogDebug("Advanced book search - Title: {Title}, Author: {Author}, Language: {Language}, Years: {YearFrom}-{YearTo}",
                title, author, language, yearFrom, yearTo);

            var allBooks = await _bookDao.GetAllAsync();

            // Apply filters
            var filteredBooks = allBooks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                filteredBooks = filteredBooks.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(language))
                filteredBooks = filteredBooks.Where(b => b.Language != null && b.Language.Contains(language, StringComparison.OrdinalIgnoreCase));

            if (yearFrom.HasValue)
                filteredBooks = filteredBooks.Where(b => b.PublicationYear >= yearFrom);

            if (yearTo.HasValue)
                filteredBooks = filteredBooks.Where(b => b.PublicationYear <= yearTo);

            return filteredBooks.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in advanced book search");
            throw new InvalidOperationException("Failed to perform advanced book search", ex);
        }
    }

    /// <summary>
    /// Get books by author ID using the junction table
    /// </summary>
    public async Task<List<Book>> GetBooksByAuthorAsync(Guid authorId)
    {
        try
        {
            _logger.LogDebug("Retrieving books by author ID: {AuthorId}", authorId);
            return await _bookDao.GetByAuthorIdAsync(authorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving books by author ID: {AuthorId}", authorId);
            throw new InvalidOperationException($"Failed to retrieve books by author: {authorId}", ex);
        }
    }
}
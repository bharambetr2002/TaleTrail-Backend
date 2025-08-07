using TaleTrail.API.Model;
using TaleTrail.API.Services;
using Microsoft.Extensions.Logging;
using static Supabase.Postgrest.Constants;

namespace TaleTrail.API.DAO;

public class BookDao
{
    private readonly SupabaseService _supabaseService;
    private readonly ILogger<BookDao> _logger;

    public BookDao(SupabaseService supabaseService, ILogger<BookDao> logger)
    {
        _supabaseService = supabaseService;
        _logger = logger;
    }

    public async Task<List<Book>> GetAllAsync()
    {
        try
        {
            _logger.LogDebug("Fetching all books from database");

            var response = await _supabaseService.Supabase
                .From<Book>()
                .Order("created_at", Ordering.Descending)
                .Get();

            if (response?.Models == null)
            {
                _logger.LogWarning("Book query returned null response or models");
                return new List<Book>();
            }

            _logger.LogInformation("Successfully fetched {Count} books", response.Models.Count);
            return response.Models;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all books from database");
            throw new InvalidOperationException("Failed to retrieve books from database", ex);
        }
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogDebug("Fetching book with ID: {BookId}", id);

            var response = await _supabaseService.Supabase
                .From<Book>()
                .Where(b => b.Id == id)
                .Get();

            var book = response?.Models?.FirstOrDefault();

            if (book == null)
            {
                _logger.LogWarning("Book not found with ID: {BookId}", id);
            }
            else
            {
                _logger.LogDebug("Successfully fetched book: {BookTitle}", book.Title);
            }

            return book;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching book with ID: {BookId}", id);
            throw new InvalidOperationException($"Failed to retrieve book with ID: {id}", ex);
        }
    }

    public async Task<List<Book>> SearchByTitleAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.LogDebug("Empty search term provided, returning empty list");
                return new List<Book>();
            }

            _logger.LogDebug("Searching books with term: {SearchTerm}", searchTerm);

            // Use ilike if supported, else fallback to Contains (for local LINQ filtering)
            var response = await _supabaseService.Supabase
                .From<Book>()
                .Where(b => b.Title.Contains(searchTerm)) // Adjust based on SDK capability
                .Get();

            if (response?.Models == null)
            {
                _logger.LogWarning("Book search returned null response");
                return new List<Book>();
            }

            _logger.LogInformation("Found {Count} books matching search term: {SearchTerm}",
                response.Models.Count, searchTerm);

            return response.Models;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching books with term: {SearchTerm}", searchTerm);
            throw new InvalidOperationException($"Failed to search books with term: {searchTerm}", ex);
        }
    }

    public async Task<Book> CreateAsync(Book book)
    {
        try
        {
            _logger.LogDebug("Creating new book: {BookTitle}", book.Title);

            var response = await _supabaseService.Supabase
                .From<Book>()
                .Insert(book);

            var createdBook = response?.Models?.FirstOrDefault();

            if (createdBook == null)
            {
                throw new InvalidOperationException("Failed to create book - no response from database");
            }

            _logger.LogInformation("Successfully created book with ID: {BookId}", createdBook.Id);
            return createdBook;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating book: {BookTitle}", book.Title);
            throw new InvalidOperationException("Failed to create book in database", ex);
        }
    }

    public async Task<List<Book>> GetByAuthorIdAsync(Guid authorId)
    {
        try
        {
            var client = _supabaseService.Supabase;

            _logger.LogDebug("Fetching BookAuthor mappings for author ID: {AuthorId}", authorId);

            // Step 1: Get all BookAuthor links for the given author
            var bookAuthorResponse = await client
                .From<BookAuthor>()
                .Where(ba => ba.AuthorId == authorId)
                .Get();

            var bookIds = bookAuthorResponse.Models.Select(ba => ba.BookId).Distinct().ToList();

            if (!bookIds.Any())
            {
                _logger.LogWarning("No books found for author ID: {AuthorId}", authorId);
                return new List<Book>();
            }

            _logger.LogDebug("Fetching books with {Count} IDs", bookIds.Count);

            // Step 2: Get all books and filter in memory (since .In() isn't supported)
            var bookResponse = await client
                .From<Book>()
                .Order("created_at", Ordering.Descending)
                .Get();

            var filteredBooks = bookResponse.Models
                .Where(b => bookIds.Contains(b.Id))
                .ToList();

            _logger.LogInformation("Returning {Count} books for author ID: {AuthorId}", filteredBooks.Count, authorId);
            return filteredBooks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving books by author ID: {AuthorId}", authorId);
            throw new InvalidOperationException($"Failed to retrieve books by author: {authorId}", ex);
        }
    }
}
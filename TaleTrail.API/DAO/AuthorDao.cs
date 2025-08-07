using TaleTrail.API.Model;
using TaleTrail.API.Services;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.DAO;

public class AuthorDao
{
    private readonly SupabaseService _supabaseService;
    private readonly ILogger<AuthorDao> _logger;

    public AuthorDao(SupabaseService supabaseService, ILogger<AuthorDao> logger)
    {
        _supabaseService = supabaseService;
        _logger = logger;
    }

    public async Task<List<Author>> GetAllAsync()
    {
        try
        {
            _logger.LogDebug("Fetching all authors from database");
            var response = await _supabaseService.Supabase.From<Author>().Get();

            if (response?.Models == null)
            {
                _logger.LogWarning("Author query returned null response or models");
                return new List<Author>();
            }

            _logger.LogInformation("Successfully fetched {Count} authors", response.Models.Count);
            return response.Models;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all authors from database");
            throw new InvalidOperationException("Failed to retrieve authors from database", ex);
        }
    }

    public async Task<Author?> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogDebug("Fetching author with ID: {AuthorId}", id);
            var response = await _supabaseService.Supabase
                .From<Author>()
                .Where(a => a.Id == id)
                .Get();

            var author = response?.Models?.FirstOrDefault();

            if (author == null)
            {
                _logger.LogWarning("Author not found with ID: {AuthorId}", id);
            }
            else
            {
                _logger.LogDebug("Successfully fetched author: {AuthorName}", author.Name);
            }

            return author;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching author with ID: {AuthorId}", id);
            throw new InvalidOperationException($"Failed to retrieve author with ID: {id}", ex);
        }
    }

    public async Task<Author> CreateAsync(Author author)
    {
        try
        {
            _logger.LogDebug("Creating new author: {AuthorName}", author.Name);

            var response = await _supabaseService.Supabase.From<Author>().Insert(author);

            if (response?.Models?.FirstOrDefault() == null)
            {
                throw new InvalidOperationException("Failed to create author - no response from database");
            }

            var createdAuthor = response.Models.First();
            _logger.LogInformation("Successfully created author with ID: {AuthorId}", createdAuthor.Id);

            return createdAuthor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating author: {AuthorName}", author.Name);
            throw new InvalidOperationException("Failed to create author in database", ex);
        }
    }
}
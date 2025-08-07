using TaleTrail.API.Model;
using TaleTrail.API.Services;
using Postgrest.Responses;

namespace TaleTrail.API.Data;

public class DataSeeder
{
    private readonly SupabaseService _supabaseService;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(SupabaseService supabaseService, ILogger<DataSeeder> logger)
    {
        _supabaseService = supabaseService;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            var existingAuthors = await _supabaseService.Supabase.From<Author>().Get();
            if (existingAuthors.Models.Count > 0)
            {
                _logger.LogInformation("📦 Database already seeded.");
                return;
            }

            _logger.LogInformation("🌱 Starting database seeding...");

            var authors = await SeedAuthorsAsync();
            _logger.LogInformation($"✅ Seeded {authors.Count} authors.");

            var publishers = await SeedPublishersAsync();
            _logger.LogInformation($"✅ Seeded {publishers.Count} publishers.");

            var books = await SeedBooksAsync(authors, publishers);
            _logger.LogInformation($"✅ Seeded {books.Count} books.");

            _logger.LogInformation("🎉 Database seeding completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Seeding failed: {Message}", ex.Message);
        }
    }

    private async Task<List<Author>> SeedAuthorsAsync()
    {
        var authors = new List<Author>
        {
            new() { Id = Guid.NewGuid(), Name = "J.K. Rowling", Bio = "British author, best known for Harry Potter.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "George R.R. Martin", Bio = "Author of A Song of Ice and Fire.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        foreach (var author in authors)
        {
            await _supabaseService.Supabase.From<Author>().Insert(author);
        }

        return authors;
    }

    private async Task<List<Publisher>> SeedPublishersAsync()
    {
        var publishers = new List<Publisher>
        {
            new() { Id = Guid.NewGuid(), Name = "Bloomsbury", Address = "London, UK", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Bantam Books", Address = "New York, USA", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        foreach (var publisher in publishers)
        {
            await _supabaseService.Supabase.From<Publisher>().Insert(publisher);
        }

        return publishers;
    }

    private async Task<List<Book>> SeedBooksAsync(List<Author> authors, List<Publisher> publishers)
    {
        var books = new List<Book>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Harry Potter and the Philosopher's Stone",
                Description = "Fantasy novel about a young wizard.",
                Language = "English",
                CoverImageUrl = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AuthorId = authors[0].Id,
                PublisherId = publishers[0].Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "A Game of Thrones",
                Description = "First book in A Song of Ice and Fire series.",
                Language = "English",
                CoverImageUrl = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AuthorId = authors[1].Id,
                PublisherId = publishers[1].Id
            }
        };

        foreach (var book in books)
        {
            await _supabaseService.Supabase.From<Book>().Insert(book);
        }

        return books;
    }
}
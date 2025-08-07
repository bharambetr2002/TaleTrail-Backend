using TaleTrail.API.Model;
using TaleTrail.API.Services;
using Microsoft.Extensions.Logging;

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
            _logger.LogInformation("🌱 Starting database seeding...");

            // Check if already seeded
            var existingBooks = await _supabaseService.Supabase.From<Book>().Get();
            if (existingBooks.Models.Any())
            {
                _logger.LogInformation("✅ Database already seeded. Skipping.");
                return;
            }

            var authors = await SeedAuthorsAsync();
            _logger.LogInformation($"✅ Seeded {authors.Count} authors");

            var publishers = await SeedPublishersAsync();
            _logger.LogInformation($"✅ Seeded {publishers.Count} publishers");

            var books = await SeedBooksAsync(authors, publishers);
            _logger.LogInformation($"✅ Seeded {books.Count} books");

            _logger.LogInformation("🎉 Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError("❌ Database seeding failed: {0}", ex.Message);
            throw;
        }
    }

    private async Task<List<Author>> SeedAuthorsAsync()
    {
        var authors = new List<Author>
        {
            new() { Name = "George Orwell" },
            new() { Name = "J.K. Rowling" },
            new() { Name = "Haruki Murakami" }
        };

        var inserted = new List<Author>();
        foreach (var author in authors)
        {
            var response = await _supabaseService.Supabase.From<Author>().Insert(author);
            inserted.Add(response.Models.First());
        }

        return inserted;
    }

    private async Task<List<Publisher>> SeedPublishersAsync()
    {
        var publishers = new List<Publisher>
        {
            new() { Name = "Penguin Books" },
            new() { Name = "Bloomsbury" }
        };

        var inserted = new List<Publisher>();
        foreach (var publisher in publishers)
        {
            var response = await _supabaseService.Supabase.From<Publisher>().Insert(publisher);
            inserted.Add(response.Models.First());
        }

        return inserted;
    }

    private async Task<List<Book>> SeedBooksAsync(List<Author> authors, List<Publisher> publishers)
    {
        var books = new List<Book>
        {
            new()
            {
                Title = "1984",
                Description = "A dystopian novel.",
                Language = "English",
                PublicationYear = 1949,
                AuthorId = authors[0].Id,
                PublisherId = publishers[0].Id
            },
            new()
            {
                Title = "Harry Potter and the Sorcerer's Stone",
                Description = "Fantasy novel for young readers.",
                Language = "English",
                PublicationYear = 1997,
                AuthorId = authors[1].Id,
                PublisherId = publishers[1].Id
            },
            new()
            {
                Title = "Kafka on the Shore",
                Description = "Magical realism and surrealist elements.",
                Language = "Japanese",
                PublicationYear = 2002,
                AuthorId = authors[2].Id,
                PublisherId = publishers[0].Id
            }
        };

        var inserted = new List<Book>();
        foreach (var book in books)
        {
            var response = await _supabaseService.Supabase.From<Book>().Insert(book);
            inserted.Add(response.Models.First());
        }

        return inserted;
    }
}
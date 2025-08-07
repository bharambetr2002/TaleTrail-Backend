// File: Data/DataSeeder.cs
using TaleTrail.API.Model;
using TaleTrail.API.Services;

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
            var response = await _supabaseService.Supabase.From<Book>().Get();

            if (response.Models.Count > 0)
            {
                _logger.LogInformation("Database already seeded.");
                return;
            }

            var sampleBooks = new List<Book>
            {
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "The Pragmatic Programmer",
                    Description = "Your Journey to Mastery",
                    CoverUrl = "https://example.com/pragmatic.jpg",
                    PublicationYear = 1999,
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Clean Code",
                    Description = "A Handbook of Agile Software Craftsmanship",
                    CoverUrl = "https://example.com/cleancode.jpg",
                    PublicationYear = 2008,
                    CreatedAt = DateTime.UtcNow
                }
            };

            foreach (var book in sampleBooks)
            {
                await _supabaseService.Supabase.From<Book>().Insert(book);
            }

            _logger.LogInformation("Database seeded with sample books.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while seeding the database.");
        }
    }
}
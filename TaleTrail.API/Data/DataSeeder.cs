using TaleTrail.API.Model.Entities;
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

            // Check if already seeded by looking at authors table
            var existingAuthors = await _supabaseService.Supabase.From<Author>().Get();
            if (existingAuthors.Models.Any())
            {
                _logger.LogInformation("✅ Database already seeded. Skipping.");
                return;
            }

            // Seed in correct order due to foreign key constraints
            var authors = await SeedAuthorsAsync();
            _logger.LogInformation($"✅ Seeded {authors.Count} authors");

            var publishers = await SeedPublishersAsync();
            _logger.LogInformation($"✅ Seeded {publishers.Count} publishers");

            var books = await SeedBooksAsync(authors, publishers);
            _logger.LogInformation($"✅ Seeded {books.Count} books");

            // Seed book-author relationships
            await SeedBookAuthorsAsync(books, authors);
            _logger.LogInformation($"✅ Seeded book-author relationships");

            _logger.LogInformation("🎉 Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Database seeding failed: {Message}", ex.Message);
            throw;
        }
    }

    private async Task<List<Author>> SeedAuthorsAsync()
    {
        var authors = new List<Author>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "George Orwell",
                Bio = "English novelist and journalist, best known for his dystopian novels.",
                BirthDate = new DateTime(1903, 6, 25),
                DeathDate = new DateTime(1950, 1, 21),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "J.K. Rowling",
                Bio = "British author, best known for the Harry Potter fantasy series.",
                BirthDate = new DateTime(1965, 7, 31),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Haruki Murakami",
                Bio = "Japanese writer known for his surreal, melancholic stories.",
                BirthDate = new DateTime(1949, 1, 12),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Toni Morrison",
                Bio = "American novelist, essayist, editor, teacher, and professor emeritus.",
                BirthDate = new DateTime(1931, 2, 18),
                DeathDate = new DateTime(2019, 8, 5),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Gabriel García Márquez",
                Bio = "Colombian novelist, short-story writer, screenwriter, and journalist.",
                BirthDate = new DateTime(1927, 3, 6),
                DeathDate = new DateTime(2014, 4, 17),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
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
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Penguin Books",
                Description = "British publishing house founded in 1935.",
                Address = "London, UK",
                FoundedYear = 1935,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Bloomsbury Publishing",
                Description = "Independent publishing house founded in 1986.",
                Address = "London, UK",
                FoundedYear = 1986,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Vintage Books",
                Description = "American trade paperbook publisher.",
                Address = "New York, USA",
                FoundedYear = 1954,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Random House",
                Description = "American book publisher and the largest general-interest paperback publisher.",
                Address = "New York, USA",
                FoundedYear = 1927,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
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
                Id = Guid.NewGuid(),
                Title = "1984",
                Description = "A dystopian social science fiction novel exploring themes of totalitarianism, surveillance, and rebellion.",
                Language = "English",
                PublicationYear = 1949,
                AuthorId = authors.First(a => a.Name == "George Orwell").Id,
                PublisherId = publishers.First(p => p.Name == "Penguin Books").Id,
                CoverImageUrl = "https://images.example.com/1984.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Animal Farm",
                Description = "An allegorical novella about a group of farm animals who rebel against their human farmer.",
                Language = "English",
                PublicationYear = 1945,
                AuthorId = authors.First(a => a.Name == "George Orwell").Id,
                PublisherId = publishers.First(p => p.Name == "Penguin Books").Id,
                CoverImageUrl = "https://images.example.com/animal-farm.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Harry Potter and the Philosopher's Stone",
                Description = "The first novel in the Harry Potter series about a young wizard's journey.",
                Language = "English",
                PublicationYear = 1997,
                AuthorId = authors.First(a => a.Name == "J.K. Rowling").Id,
                PublisherId = publishers.First(p => p.Name == "Bloomsbury Publishing").Id,
                CoverImageUrl = "https://images.example.com/hp1.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Kafka on the Shore",
                Description = "A 2002 novel combining elements of magical realism and postmodernism.",
                Language = "Japanese",
                PublicationYear = 2002,
                AuthorId = authors.First(a => a.Name == "Haruki Murakami").Id,
                PublisherId = publishers.First(p => p.Name == "Vintage Books").Id,
                CoverImageUrl = "https://images.example.com/kafka-shore.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Beloved",
                Description = "A 1987 novel about the life of a former slave after the American Civil War.",
                Language = "English",
                PublicationYear = 1987,
                AuthorId = authors.First(a => a.Name == "Toni Morrison").Id,
                PublisherId = publishers.First(p => p.Name == "Random House").Id,
                CoverImageUrl = "https://images.example.com/beloved.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "One Hundred Years of Solitude",
                Description = "A landmark novel telling the multi-generational story of the Buendía family.",
                Language = "Spanish",
                PublicationYear = 1967,
                AuthorId = authors.First(a => a.Name == "Gabriel García Márquez").Id,
                PublisherId = publishers.First(p => p.Name == "Random House").Id,
                CoverImageUrl = "https://images.example.com/solitude.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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

    private async Task SeedBookAuthorsAsync(List<Book> books, List<Author> authors)
    {
        // Create book-author relationships (many-to-many)
        var bookAuthors = new List<BookAuthor>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BookId = books.First(b => b.Title == "Kafka on the Shore").Id,
                AuthorId = authors.First(a => a.Name == "Haruki Murakami").Id,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                BookId = books.First(b => b.Title == "Beloved").Id,
                AuthorId = authors.First(a => a.Name == "Toni Morrison").Id,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                BookId = books.First(b => b.Title == "One Hundred Years of Solitude").Id,
                AuthorId = authors.First(a => a.Name == "Gabriel García Márquez").Id,
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var bookAuthor in bookAuthors)
        {
            await _supabaseService.Supabase.From<BookAuthor>().Insert(bookAuthor);
        }
    }
}1984").Id,
                AuthorId = authors.First(a => a.Name == "George Orwell").Id,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                BookId = books.First(b => b.Title == "Animal Farm").Id,
                AuthorId = authors.First(a => a.Name == "George Orwell").Id,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                BookId = books.First(b => b.Title == "Harry Potter and the Philosopher's Stone").Id,
                AuthorId = authors.First(a => a.Name == "J.K. Rowling").Id,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                BookId = books.First(b => b.Title == "
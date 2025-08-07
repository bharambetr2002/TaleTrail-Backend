using TaleTrail.API.Model;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO;

public class AuthorDao
{
    private readonly SupabaseService _supabaseService;

    public AuthorDao(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<List<Author>> GetAllAsync()
    {
        var response = await _supabaseService.Supabase.From<Author>().Get();
        return response.Models ?? new List<Author>();
    }

    public async Task<Author?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseService.Supabase
            .From<Author>()
            .Where(a => a.Id == id)
            .Get();
        return response.Models?.FirstOrDefault();
    }

    public async Task<List<Author>> GetByBookIdAsync(Guid bookId)
    {
        // Get book-author relationships
        var bookAuthorResponse = await _supabaseService.Supabase
            .From<BookAuthor>()
            .Where(ba => ba.BookId == bookId)
            .Get();

        var authorIds = bookAuthorResponse.Models?.Select(ba => ba.AuthorId).ToList() ?? new List<Guid>();

        if (!authorIds.Any())
            return new List<Author>();

        // Get all authors and filter by IDs (since Supabase C# doesn't have .In() method)
        var allAuthorsResponse = await _supabaseService.Supabase.From<Author>().Get();
        var filteredAuthors = allAuthorsResponse.Models?
            .Where(a => authorIds.Contains(a.Id))
            .ToList() ?? new List<Author>();

        return filteredAuthors;
    }

    public async Task<Author> CreateAsync(Author author)
    {
        author.Id = Guid.NewGuid();
        author.CreatedAt = DateTime.UtcNow;
        author.UpdatedAt = DateTime.UtcNow;

        var response = await _supabaseService.Supabase.From<Author>().Insert(author);
        return response.Models.First();
    }
}
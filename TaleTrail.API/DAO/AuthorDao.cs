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
        return response.Models;
    }

    public async Task<Author?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseService.Supabase
            .From<Author>()
            .Where(a => a.Id == id)
            .Get();

        return response.Models.FirstOrDefault();
    }

    public async Task<Author> CreateAsync(Author author)
    {
        var response = await _supabaseService.Supabase.From<Author>().Insert(author);
        return response.Models.First();
    }
}
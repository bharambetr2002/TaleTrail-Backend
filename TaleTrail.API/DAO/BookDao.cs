using TaleTrail.API.Model;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO;

public class BookDao
{
    private readonly SupabaseService _supabaseService;

    public BookDao(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<List<Book>> GetAllAsync()
    {
        var response = await _supabaseService.Supabase.From<Book>().Get();
        return response.Models;
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseService.Supabase
            .From<Book>()
            .Where(b => b.Id == id)
            .Get();

        return response.Models.FirstOrDefault();
    }

    public async Task<List<Book>> SearchByTitleAsync(string searchTerm)
    {
        var response = await _supabaseService.Supabase
            .From<Book>()
            .Where(b => b.Title.Contains(searchTerm))
            .Get();

        return response.Models;
    }

    public async Task<Book> CreateAsync(Book book)
    {
        var response = await _supabaseService.Supabase.From<Book>().Insert(book);
        return response.Models.First();
    }
}
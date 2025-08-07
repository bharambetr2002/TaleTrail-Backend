// File: DAO/BookDao.cs
using Postgrest;
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
}
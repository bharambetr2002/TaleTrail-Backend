using TaleTrail.API.Model;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO;

public class PublisherDao
{
    private readonly SupabaseService _supabaseService;

    public PublisherDao(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<List<Publisher>> GetAllAsync()
    {
        var response = await _supabaseService.Supabase.From<Publisher>().Get();
        return response.Models;
    }

    public async Task<Publisher?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseService.Supabase
            .From<Publisher>()
            .Where(p => p.Id == id)
            .Get();

        return response.Models.FirstOrDefault();
    }

    public async Task<Publisher> CreateAsync(Publisher publisher)
    {
        var response = await _supabaseService.Supabase.From<Publisher>().Insert(publisher);
        return response.Models.First();
    }
}
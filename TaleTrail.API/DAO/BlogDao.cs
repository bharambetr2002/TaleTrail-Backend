using TaleTrail.API.Model;
using TaleTrail.API.Services;
using Supabase.Postgrest;

namespace TaleTrail.API.DAO;

public class BlogDao
{
    private readonly SupabaseService _supabaseService;

    public BlogDao(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<List<Blog>> GetAllAsync()
    {
        var response = await _supabaseService.Supabase
            .From<Blog>()
            .Order("created_at", Constants.Ordering.Descending)
            .Get();

        return response.Models;
    }

    public async Task<List<Blog>> GetByUserIdAsync(Guid userId)
    {
        var response = await _supabaseService.Supabase
            .From<Blog>()
            .Where(b => b.UserId == userId)
            .Order("created_at", Constants.Ordering.Descending)
            .Get();

        return response.Models;
    }

    public async Task<Blog?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseService.Supabase
            .From<Blog>()
            .Where(b => b.Id == id)
            .Get();

        return response.Models.FirstOrDefault();
    }

    public async Task<Blog> CreateAsync(Blog blog)
    {
        var response = await _supabaseService.Supabase.From<Blog>().Insert(blog);
        return response.Models.First();
    }

    public async Task<Blog> UpdateAsync(Blog blog)
    {
        blog.UpdatedAt = DateTime.UtcNow;
        var response = await _supabaseService.Supabase
            .From<Blog>()
            .Where(b => b.Id == blog.Id)
            .Update(blog);

        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseService.Supabase
            .From<Blog>()
            .Where(b => b.Id == id)
            .Delete();
    }
}
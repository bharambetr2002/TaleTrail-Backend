using TaleTrail.API.Model;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO;

public class BlogLikeDao
{
    private readonly SupabaseService _supabaseService;

    public BlogLikeDao(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<BlogLike?> GetByBlogAndUserAsync(Guid blogId, Guid userId)
    {
        var response = await _supabaseService.Supabase
            .From<BlogLike>()
            .Where(bl => bl.BlogId == blogId && bl.UserId == userId)
            .Get();

        return response.Models.FirstOrDefault();
    }

    public async Task<int> GetLikeCountAsync(Guid blogId)
    {
        var response = await _supabaseService.Supabase
            .From<BlogLike>()
            .Where(bl => bl.BlogId == blogId)
            .Get();

        return response.Models.Count;
    }

    public async Task<BlogLike> CreateAsync(BlogLike blogLike)
    {
        var response = await _supabaseService.Supabase.From<BlogLike>().Insert(blogLike);
        return response.Models.First();
    }

    public async Task DeleteAsync(Guid blogId, Guid userId)
    {
        await _supabaseService.Supabase
            .From<BlogLike>()
            .Where(bl => bl.BlogId == blogId && bl.UserId == userId)
            .Delete();
    }
}
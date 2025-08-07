using TaleTrail.API.Model;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO;

public class ReviewDao
{
    private readonly SupabaseService _supabaseService;

    public ReviewDao(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<List<Review>> GetByBookIdAsync(Guid bookId)
    {
        var response = await _supabaseService.Supabase
            .From<Review>()
            .Where(r => r.BookId == bookId)
            .Get();

        return response.Models;
    }

    public async Task<Review?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseService.Supabase
            .From<Review>()
            .Where(r => r.Id == id)
            .Get();

        return response.Models.FirstOrDefault();
    }

    public async Task<List<Review>> GetByUserIdAsync(Guid userId)
    {
        var response = await _supabaseService.Supabase
            .From<Review>()
            .Where(r => r.UserId == userId)
            .Get();

        return response.Models;
    }

    public async Task<Review> CreateAsync(Review review)
    {
        var response = await _supabaseService.Supabase.From<Review>().Insert(review);
        return response.Models.First();
    }

    public async Task<Review> UpdateAsync(Review review)
    {
        review.UpdatedAt = DateTime.UtcNow;
        var response = await _supabaseService.Supabase
            .From<Review>()
            .Where(r => r.Id == review.Id)
            .Update(review);

        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseService.Supabase
            .From<Review>()
            .Where(r => r.Id == id)
            .Delete();
    }
}
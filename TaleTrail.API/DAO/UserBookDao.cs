using TaleTrail.API.Model;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO;

public class UserBookDao
{
    private readonly SupabaseService _supabaseService;

    public UserBookDao(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<List<UserBook>> GetByUserIdAsync(Guid userId)
    {
        var response = await _supabaseService.Supabase
            .From<UserBook>()
            .Where(ub => ub.UserId == userId)
            .Get();

        return response.Models;
    }

    public async Task<UserBook?> GetByUserAndBookAsync(Guid userId, Guid bookId)
    {
        var response = await _supabaseService.Supabase
            .From<UserBook>()
            .Where(ub => ub.UserId == userId && ub.BookId == bookId)
            .Get();

        return response.Models.FirstOrDefault();
    }

    public async Task<List<UserBook>> GetInProgressByUserAsync(Guid userId)
    {
        var response = await _supabaseService.Supabase
            .From<UserBook>()
            .Where(ub => ub.UserId == userId && ub.ReadingStatus == "InProgress")
            .Get();

        return response.Models;
    }

    public async Task<UserBook> CreateAsync(UserBook userBook)
    {
        var response = await _supabaseService.Supabase.From<UserBook>().Insert(userBook);
        return response.Models.First();
    }

    public async Task<UserBook> UpdateAsync(UserBook userBook)
    {
        userBook.UpdatedAt = DateTime.UtcNow;
        var response = await _supabaseService.Supabase
            .From<UserBook>()
            .Where(ub => ub.Id == userBook.Id)
            .Update(userBook);

        return response.Models.First();
    }

    public async Task DeleteAsync(Guid userId, Guid bookId)
    {
        await _supabaseService.Supabase
            .From<UserBook>()
            .Where(ub => ub.UserId == userId && ub.BookId == bookId)
            .Delete();
    }
}
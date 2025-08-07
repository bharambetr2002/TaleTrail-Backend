using TaleTrail.API.Model;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO;

public class UserDao
{
    private readonly SupabaseService _supabaseService;

    public UserDao(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseService.Supabase
            .From<User>()
            .Where(u => u.Id == id)
            .Get();

        return response.Models.FirstOrDefault();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var response = await _supabaseService.Supabase
            .From<User>()
            .Where(u => u.Username == username)
            .Get();

        return response.Models.FirstOrDefault();
    }

    public async Task<User> CreateAsync(User user)
    {
        var response = await _supabaseService.Supabase.From<User>().Insert(user);
        return response.Models.First();
    }

    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        var response = await _supabaseService.Supabase
            .From<User>()
            .Where(u => u.Id == user.Id)
            .Update(user);

        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseService.Supabase
            .From<User>()
            .Where(u => u.Id == id)
            .Delete();
    }
}
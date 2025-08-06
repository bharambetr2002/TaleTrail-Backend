using TaleTrail.API.Models;
using TaleTrail.API.DTOs;

namespace TaleTrail.API.Services
{
    public class UserService
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<UserService> _logger;

        public UserService(SupabaseService supabase, ILogger<UserService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var response = await _supabase.Client.From<User>().Get();
            return response.Models ?? new List<User>();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var response = await _supabase.Client.From<User>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();
            return response.Models?.FirstOrDefault();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var response = await _supabase.Client.From<User>()
                .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
                .Get();
            return response.Models?.FirstOrDefault();
        }

        public async Task<User?> UpdateAsync(Guid id, UserDto dto, Guid userId)
        {
            if (id != userId) return null;
            var response = await _supabase.Client.From<User>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();
            var user = response.Models?.FirstOrDefault();
            if (user == null) return null;
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.AvatarUrl = dto.AvatarUrl;
            user.Bio = dto.Bio;
            var updateResponse = await _supabase.Client.From<User>().Update(user);
            return updateResponse.Models?.FirstOrDefault();
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            if (id != userId) return false;
            var user = new User { Id = id };
            await _supabase.Client.From<User>().Delete(user);
            return true;
        }
    }
}
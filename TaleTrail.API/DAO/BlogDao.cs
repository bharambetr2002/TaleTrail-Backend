using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO
{
    public class BlogDao
    {
        private readonly Client _client;

        public BlogDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Blog>> GetByUserId(Guid userId) =>
            (await _client.From<Blog>().Where(b => b.UserId == userId).Get()).Models;
    }
}
using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO
{
    public class CategoryDao
    {
        private readonly Client _client;

        public CategoryDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Category>> GetAll() => (await _client.From<Category>().Get()).Models;
    }
}
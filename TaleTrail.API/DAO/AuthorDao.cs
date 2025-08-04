using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO
{
    public class AuthorDao
    {
        private readonly Client _client;

        public AuthorDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Author>> GetAll() => (await _client.From<Author>().Get()).Models;
    }
}
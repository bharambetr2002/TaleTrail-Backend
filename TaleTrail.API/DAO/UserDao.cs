using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO
{
    public class UserDao
    {
        private readonly Client _client;

        public UserDao(SupabaseService service) => _client = service.Client;

        public async Task<List<User>> GetAll() => (await _client.From<User>().Get()).Models;
    }
}
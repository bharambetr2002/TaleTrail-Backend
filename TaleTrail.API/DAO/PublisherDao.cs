using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO
{
    public class PublisherDao
    {
        private readonly Client _client;

        public PublisherDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Publisher>> GetAll() => (await _client.From<Publisher>().Get()).Models;
    }
}
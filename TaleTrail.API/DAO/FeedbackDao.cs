using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO
{
    public class FeedbackDao
    {
        private readonly Client _client;

        public FeedbackDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Feedback>> GetAll() => (await _client.From<Feedback>().Get()).Models;
    }
}
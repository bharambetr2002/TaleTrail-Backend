using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO
{
    public class SubscriptionDao
    {
        private readonly Client _client;

        public SubscriptionDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Subscription>> GetByUserId(Guid userId) =>
            (await _client.From<Subscription>().Where(s => s.UserId == userId).Get()).Models;
    }
}
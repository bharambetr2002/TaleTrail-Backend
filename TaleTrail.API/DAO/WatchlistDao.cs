using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO
{
    public class WatchlistDao
    {
        private readonly Client _client;

        public WatchlistDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Watchlist>> GetByUserId(Guid userId) =>
            (await _client.From<Watchlist>().Where(w => w.UserId == userId).Get()).Models;
    }
}
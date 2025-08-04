using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO
{
    public class ReviewDao
    {
        private readonly Client _client;

        public ReviewDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Review>> GetByBookId(Guid bookId) =>
            (await _client.From<Review>().Where(r => r.BookId == bookId).Get()).Models;
    }
}
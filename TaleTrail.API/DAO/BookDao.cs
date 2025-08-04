using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO
{
    public class BookDao
    {
        private readonly Client _client;

        public BookDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Book>> GetByUserId(Guid userId) =>
            (await _client.From<Book>().Where(b => b.UserId == userId).Get()).Models;

        public async Task<Book?> Add(Book book)
        {
            var response = await _client.From<Book>().Insert(book);
            return response.Models.FirstOrDefault();
        }
    }
}
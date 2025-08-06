using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System.Threading.Tasks;
using System.Linq;

namespace TaleTrail.API.DAO
{
    public class BookAuthorDao
    {
        private readonly Client _client;

        public BookAuthorDao(SupabaseService service) => _client = service.Client;

        public async Task<BookAuthor?> AddAsync(BookAuthor bookAuthor)
        {
            var response = await _client.From<BookAuthor>().Insert(bookAuthor);
            return response.Models.FirstOrDefault();
        }

        // Optional: Add methods to get authors for a book or books for an author if needed later.
    }
}

using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TaleTrail.API.DAO
{
    public class BookDao
    {
        private readonly Client _client;

        public BookDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Book>> GetAllAsync(string? searchTerm = null)
        {
            // CORRECTED LINE: Use 'var' to let the compiler infer the correct type
            var query = _client.From<Book>();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = (Supabase.Interfaces.ISupabaseTable<Book, Supabase.Realtime.RealtimeChannel>)query.Filter("title", Supabase.Postgrest.Constants.Operator.ILike, $"%{searchTerm}%");
            }
            var response = await query.Get();
            return response.Models;
        }

        public async Task<Book?> GetByIdAsync(Guid id)
        {
            var response = await _client.From<Book>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Single();
            return response;
        }

        public async Task<Book?> AddAsync(Book book)
        {
            var response = await _client.From<Book>().Insert(book);
            return response.Models.FirstOrDefault();
        }

        public async Task<Book?> UpdateAsync(Book book)
        {
            var response = await _client.From<Book>().Update(book);
            return response.Models.FirstOrDefault();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _client.From<Book>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString()).Delete();
        }
    }
}
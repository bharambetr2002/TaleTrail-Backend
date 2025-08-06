using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TaleTrail.API.DAO
{
    public class AuthorDao
    {
        private readonly Client _client;

        public AuthorDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Author>> GetAllAsync(string? searchTerm = null)
        {
            // CORRECTED LINE: Use 'var' to let the compiler infer the correct type
            var query = _client.From<Author>();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = (Supabase.Interfaces.ISupabaseTable<Author, Supabase.Realtime.RealtimeChannel>)query.Filter("name", Supabase.Postgrest.Constants.Operator.ILike, $"%{searchTerm}%");
            }
            var response = await query.Get();
            return response.Models;
        }

        public async Task<Author?> GetByIdAsync(Guid id)
        {
            var response = await _client.From<Author>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Single();
            return response;
        }

        public async Task<Author?> AddAsync(Author author)
        {
            var response = await _client.From<Author>().Insert(author);
            return response.Models.FirstOrDefault();
        }

        public async Task<Author?> UpdateAsync(Author author)
        {
            var response = await _client.From<Author>().Update(author);
            return response.Models.FirstOrDefault();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _client.From<Author>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString()).Delete();
        }
    }
}
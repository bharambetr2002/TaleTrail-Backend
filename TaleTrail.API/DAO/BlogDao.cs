using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TaleTrail.API.DAO
{
    public class BlogDao
    {
        private readonly Client _client;

        public BlogDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Blog>> GetAllAsync(Guid? userId = null)
        {
            // CORRECTED LINE: Use 'var' to let the compiler infer the correct type
            var query = _client.From<Blog>();
            if (userId.HasValue)
            {
                query = (Supabase.Interfaces.ISupabaseTable<Blog, Supabase.Realtime.RealtimeChannel>)query.Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.Value.ToString());
            }
            var response = await query.Get();
            return response.Models;
        }

        public async Task<Blog?> GetByIdAsync(Guid id)
        {
            var response = await _client.From<Blog>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Single();
            return response;
        }

        public async Task<Blog?> AddAsync(Blog blog)
        {
            var response = await _client.From<Blog>().Insert(blog);
            return response.Models.FirstOrDefault();
        }

        public async Task<Blog?> UpdateAsync(Blog blog)
        {
            var response = await _client.From<Blog>().Update(blog);
            return response.Models.FirstOrDefault();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _client.From<Blog>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString()).Delete();
        }
    }
}
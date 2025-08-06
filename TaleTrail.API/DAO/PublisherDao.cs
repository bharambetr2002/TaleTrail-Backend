using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TaleTrail.API.DAO
{
    public class PublisherDao
    {
        private readonly Client _client;

        public PublisherDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Publisher>> GetAllAsync()
        {
            var response = await _client.From<Publisher>().Get();
            return response.Models;
        }

        public async Task<Publisher?> GetByIdAsync(Guid id)
        {
            var response = await _client.From<Publisher>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Single();
            return response;
        }

        public async Task<Publisher?> AddAsync(Publisher publisher)
        {
            var response = await _client.From<Publisher>().Insert(publisher);
            return response.Models.FirstOrDefault();
        }

        public async Task<Publisher?> UpdateAsync(Publisher publisher)
        {
            var response = await _client.From<Publisher>().Update(publisher);
            return response.Models.FirstOrDefault();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _client.From<Publisher>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString()).Delete();
        }
    }
}
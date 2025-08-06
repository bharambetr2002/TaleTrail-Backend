using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TaleTrail.API.DAO
{
    public class ReviewDao
    {
        private readonly Client _client;

        public ReviewDao(SupabaseService service) => _client = service.Client;

        public async Task<List<Review>> GetByBookIdAsync(Guid bookId)
        {
            var response = await _client.From<Review>()
                .Filter("book_id", Supabase.Postgrest.Constants.Operator.Equals, bookId.ToString())
                .Get();
            return response.Models;
        }

        public async Task<List<Review>> GetByUserIdAsync(Guid userId)
        {
            var response = await _client.From<Review>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();
            return response.Models;
        }

        public async Task<Review?> GetByIdAsync(Guid id)
        {
            var response = await _client.From<Review>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Single();
            return response;
        }

        public async Task<Review?> AddAsync(Review review)
        {
            var response = await _client.From<Review>().Insert(review);
            return response.Models.FirstOrDefault();
        }

        public async Task<Review?> UpdateAsync(Review review)
        {
            var response = await _client.From<Review>().Update(review);
            return response.Models.FirstOrDefault();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _client.From<Review>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString()).Delete();
        }
    }
}
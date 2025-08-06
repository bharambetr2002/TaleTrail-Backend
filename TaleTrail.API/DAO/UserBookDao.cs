using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TaleTrail.API.DAO
{
    public class UserBookDao
    {
        private readonly Client _client;

        public UserBookDao(SupabaseService service) => _client = service.Client;

        public async Task<List<UserBook>> GetByUserIdAsync(Guid userId)
        {
            var response = await _client.From<UserBook>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();
            return response.Models;
        }

        public async Task<UserBook?> GetByUserAndBookAsync(Guid userId, Guid bookId)
        {
            var response = await _client.From<UserBook>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("book_id", Supabase.Postgrest.Constants.Operator.Equals, bookId.ToString())
                .Single();
            return response;
        }

        public async Task<int> GetInProgressCountAsync(Guid userId)
        {
            var response = await _client.From<UserBook>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("status", Supabase.Postgrest.Constants.Operator.Equals, "in_progress")
                .Count(Supabase.Postgrest.Constants.CountType.Exact);

            return (int)response;
        }

        public async Task<UserBook?> AddAsync(UserBook userBook)
        {
            var response = await _client.From<UserBook>().Insert(userBook);
            return response.Models.FirstOrDefault();
        }

        public async Task<UserBook?> UpdateAsync(UserBook userBook)
        {
            var response = await _client.From<UserBook>().Update(userBook);
            return response.Models.FirstOrDefault();
        }

        public async Task DeleteAsync(Guid userId, Guid bookId)
        {
            await _client.From<UserBook>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("book_id", Supabase.Postgrest.Constants.Operator.Equals, bookId.ToString())
                .Delete();
        }
    }
}
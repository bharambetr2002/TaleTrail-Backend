using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TaleTrail.API.DAO
{
    public class UserDao
    {
        private readonly Client _client;

        public UserDao(SupabaseService service) => _client = service.Client;

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var response = await _client.From<User>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Single();
            return response;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var response = await _client.From<User>()
                .Filter("username", Supabase.Postgrest.Constants.Operator.Equals, username)
                .Single();
            return response;
        }

        public async Task<User?> AddAsync(User user)
        {
            var response = await _client.From<User>().Insert(user);
            return response.Models.FirstOrDefault();
        }

        public async Task<User?> UpdateAsync(User user)
        {
            var response = await _client.From<User>().Update(user);
            return response.Models.FirstOrDefault();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _client.From<User>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString()).Delete();
        }
    }
}
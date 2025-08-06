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
            try
            {
                var response = await _client.From<User>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Single();
                return response;
            }
            catch (Exception)
            {
                // Return null if user not found
                return null;
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                var response = await _client.From<User>()
                    .Filter("username", Supabase.Postgrest.Constants.Operator.Equals, username)
                    .Single();
                return response;
            }
            catch (Exception)
            {
                // Return null if user not found
                return null;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                var response = await _client.From<User>()
                    .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
                    .Single();
                return response;
            }
            catch (Exception)
            {
                // Return null if user not found
                return null;
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var response = await _client.From<User>().Get();
                return response.Models ?? new List<User>();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            try
            {
                var user = await GetByUsernameAsync(username);
                return user != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                var user = await GetByEmailAsync(email);
                return user != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<User?> AddAsync(User user)
        {
            var response = await _client.From<User>().Insert(user);
            return response.Models?.FirstOrDefault();
        }

        public async Task<User?> UpdateAsync(User user)
        {
            var response = await _client.From<User>().Update(user);
            return response.Models?.FirstOrDefault();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _client.From<User>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString()).Delete();
        }
    }
}
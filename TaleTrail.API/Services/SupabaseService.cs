using Supabase;
using Supabase.Interfaces;
using TaleTrail.API.Models;
using Microsoft.Extensions.Configuration;

namespace TaleTrail.API.Services
{
    public class SupabaseService
    {
        private readonly Client _client;

        public SupabaseService(IConfiguration config)
        {
            var supabaseUrl = config["Supabase:Url"];
            var supabaseKey = config["Supabase:Key"];

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true
            };

            _client = new Client(supabaseUrl, supabaseKey, options);
            _client.InitializeAsync().Wait(); // Important!
        }

        public Client Client => _client;
    }
}
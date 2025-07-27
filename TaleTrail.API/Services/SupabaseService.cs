using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace TaleTrail.API.Services
{
    public class SupabaseService
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public SupabaseService(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _supabaseUrl = config["Supabase:Url"] ?? throw new ArgumentNullException("Supabase:Url");
            _supabaseKey = config["Supabase:Key"] ?? throw new ArgumentNullException("Supabase:Key");
        }
    }
}

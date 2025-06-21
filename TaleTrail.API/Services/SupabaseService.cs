using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace TaleTrail.API.Services
{
    public class SupabaseAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;

        public SupabaseAuthService(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _supabaseUrl = config["Supabase:Url"];
            _supabaseKey = config["Supabase:Key"];
        }

        public async Task<string> SignUpAsync(string email, string password)
        {
            var payload = new { email, password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);

            var response = await _httpClient.PostAsync($"{_supabaseUrl}/auth/v1/signup", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> SignInAsync(string email, string password)
        {
            var payload = new { email, password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);

            var response = await _httpClient.PostAsync($"{_supabaseUrl}/auth/v1/token?grant_type=password", content);
            return await response.Content.ReadAsStringAsync();
        }
    }
}

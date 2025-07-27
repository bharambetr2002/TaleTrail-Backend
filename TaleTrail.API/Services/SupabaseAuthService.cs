using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TaleTrail.API.DTOs.Auth;

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
            _supabaseUrl = config["Supabase:Url"] ?? throw new ArgumentNullException("Supabase:Url");
            _supabaseKey = config["Supabase:Key"] ?? throw new ArgumentNullException("Supabase:Key");
        }

        public async Task<string> SignUpAsync(AuthRequestDto request)
        {
            var url = $"{_supabaseUrl}/auth/v1/signup";
            var payload = JsonSerializer.Serialize(new { email = request.Email, password = request.Password });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("apikey", _supabaseKey);
            req.Content = content;

            var response = await _httpClient.SendAsync(req);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> SignInAsync(AuthRequestDto request)
        {
            var url = $"{_supabaseUrl}/auth/v1/token?grant_type=password";
            var payload = JsonSerializer.Serialize(new { email = request.Email, password = request.Password });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("apikey", _supabaseKey);
            req.Content = content;

            var response = await _httpClient.SendAsync(req);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
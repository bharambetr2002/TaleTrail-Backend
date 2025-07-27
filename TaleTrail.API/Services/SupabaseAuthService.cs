using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TaleTrail.API.DTOs.Auth;
using TaleTrail.API.Models; // Needed for `User` class

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

            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            req.Headers.Add("apikey", _supabaseKey);

            var response = await _httpClient.SendAsync(req);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> SignInAsync(AuthRequestDto request)
        {
            var url = $"{_supabaseUrl}/auth/v1/token?grant_type=password";
            var payload = JsonSerializer.Serialize(new { email = request.Email, password = request.Password });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            req.Headers.Add("apikey", _supabaseKey);

            var response = await _httpClient.SendAsync(req);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<User?> GetUserFromToken(string accessToken)
        {
            var url = $"{_supabaseUrl}/auth/v1/user";

            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            req.Headers.Add("apikey", _supabaseKey);

            var response = await _httpClient.SendAsync(req);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
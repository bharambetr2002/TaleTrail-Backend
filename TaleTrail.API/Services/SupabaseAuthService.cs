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
            _supabaseUrl = config["Supabase:Url"]!;
            _supabaseKey = config["Supabase:Key"]!;
        }

        public async Task<object> SignUpAsync(AuthRequestDto dto)
        {
            var payload = new
            {
                email = dto.Email,
                password = dto.Password
            };

            var response = await _httpClient.PostAsync(
                $"{_supabaseUrl}/auth/v1/signup",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            );

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content);
        }

        public async Task<object> SignInAsync(AuthRequestDto dto)
        {
            var payload = new
            {
                email = dto.Email,
                password = dto.Password
            };

            var response = await _httpClient.PostAsync(
                $"{_supabaseUrl}/auth/v1/token?grant_type=password",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            );

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content);
        }

        public async Task<UserDto?> GetUserFromToken(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_supabaseUrl}/auth/v1/user");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return new UserDto
            {
                Id = root.GetProperty("id").GetString(),
                Email = root.GetProperty("email").GetString() ?? ""
            };
        }
    }
}
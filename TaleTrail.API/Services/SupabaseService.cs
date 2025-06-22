using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace TaleTrail.API.Services
{
    /// <summary>
    /// This service handles authentication (signup & login) by communicating with Supabase Auth API.
    /// </summary>
    public class SupabaseAuthService
    {
        private readonly HttpClient _httpClient;     // HTTP client for sending requests
        private readonly string _supabaseUrl;        // Supabase project base URL
        private readonly string _supabaseKey;        // Supabase project API key

        /// <summary>
        /// Constructor to initialize the service with configuration settings.
        /// </summary>
        /// <param name="config">Application configuration to read Supabase credentials.</param>
        public SupabaseAuthService(IConfiguration config)
        {
            _httpClient = new HttpClient(); // New instance of HttpClient
            _supabaseUrl = config["Supabase:Url"]; // Read Supabase URL from config
            _supabaseKey = config["Supabase:Key"]; // Read Supabase Key from config
        }

        /// <summary>
        /// Registers a new user with the given email and password using Supabase's signup endpoint.
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <param name="password">User's password.</param>
        /// <returns>Raw JSON response from Supabase as a string.</returns>
        public async Task<string> SignUpAsync(string email, string password)
        {
            // Create request payload
            var payload = new { email, password };

            // Serialize payload to JSON and set content headers
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Clear existing headers and add Supabase API key
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);

            // Send POST request to Supabase signup endpoint
            var response = await _httpClient.PostAsync($"{_supabaseUrl}/auth/v1/signup", content);

            // Return the response body as string
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Authenticates a user with email and password using Supabase's token endpoint (password grant).
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <param name="password">User's password.</param>
        /// <returns>Raw JSON response containing access token and session info.</returns>
        public async Task<string> SignInAsync(string email, string password)
        {
            // Create login payload
            var payload = new { email, password };

            // Serialize payload and define content headers
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Clear existing headers and add Supabase API key
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);

            // Send POST request to Supabase token endpoint with grant_type=password
            var response = await _httpClient.PostAsync($"{_supabaseUrl}/auth/v1/token?grant_type=password", content);

            // Return the response content
            return await response.Content.ReadAsStringAsync();
        }
    }
}

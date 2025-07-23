using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using System.Text.Json;
using System.Net.Http.Headers;

namespace TaleTrail.API.Controllers
{
    // This marks the class as an API controller and sets the base route to "api/auth"
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SupabaseAuthService _authService;
        private readonly IConfiguration _config;

        // Constructor injection for Supabase auth service and app configuration (to access keys/URLs)
        public AuthController(SupabaseAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        /// <summary>
        /// Handles user signup.
        /// Accepts email and password from request body and sends it to Supabase signup method.
        /// </summary>
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            // Call service method to register the user in Supabase
            var result = await _authService.SignUpAsync(request.Email!, request.Password!);

            // Return the response from Supabase as JSON content
            return Content(result, "application/json");
        }

        /// <summary>
        /// Handles user login.
        /// Accepts email and password, calls Supabase sign-in API, and returns the result.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Call service method to authenticate the user in Supabase
            var result = await _authService.SignInAsync(request.Email!, request.Password!);

            // Return the response as JSON content
            return Content(result, "application/json");
        }

        /// <summary>
        /// Fetches the currently authenticated user's profile.
        /// Requires a valid JWT token in the Authorization header.
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            // Get the Authorization header from the request
            var authHeader = Request.Headers["Authorization"].ToString();

            // Validate the header format
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Missing or invalid token");

            // Extract the token from the header
            var token = authHeader.Replace("Bearer ", "");

            // Create an HTTP client to send a request to Supabase
            var httpClient = new HttpClient();

            // Set required headers: API key and Authorization token
            httpClient.DefaultRequestHeaders.Add("apikey", _config["Supabase:Key"]);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Send GET request to Supabase user endpoint to fetch current user info
            var response = await httpClient.GetAsync($"{_config["Supabase:Url"]}/auth/v1/user");

            // If the token is invalid or expired, return 401 Unauthorized
            if (!response.IsSuccessStatusCode)
                return Unauthorized("Invalid or expired token");

            // Read and return the user data as JSON
            var body = await response.Content.ReadAsStringAsync();
            return Content(body, "application/json");
        }
    }

    // DTO (Data Transfer Object) for signup requests
    public class SignupRequest
    {
        public string? Email { get; set; }      // User email
        public string? Password { get; set; }   // User password
    }

    // DTO for login requests
    public class LoginRequest
    {
        public string? Email { get; set; }      // User email
        public string? Password { get; set; }   // User password
    }
}

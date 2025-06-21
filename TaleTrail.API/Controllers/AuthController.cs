using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using System.Text.Json;
using System.Net.Http.Headers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SupabaseAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(SupabaseAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            var result = await _authService.SignUpAsync(request.Email, request.Password);
            return Content(result, "application/json");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.SignInAsync(request.Email, request.Password);
            return Content(result, "application/json");
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Missing or invalid token");

            var token = authHeader.Replace("Bearer ", "");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("apikey", _config["Supabase:Key"]);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync($"{_config["Supabase:Url"]}/auth/v1/user");

            if (!response.IsSuccessStatusCode)
                return Unauthorized("Invalid or expired token");

            var body = await response.Content.ReadAsStringAsync();
            return Content(body, "application/json");
        }
    }

    public class SignupRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
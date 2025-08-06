using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs.Auth.Signup;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDTO request)
        {
            var result = await _authService.SignupAsync(request);
            return Ok(ApiResponse<object>.SuccessResult(result, "User registered successfully"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(ApiResponse<object>.SuccessResult(result, "Login successful"));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var success = await _authService.LogoutAsync();
            return Ok(ApiResponse.SuccessResult(success ? "Logged out successfully" : "Logout failed"));
        }
    }
}
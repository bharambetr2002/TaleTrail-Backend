using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.DTOs;
using TaleTrail.API.DTOs.Auth;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SupabaseAuthService _authService;

        public AuthController(SupabaseAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] AuthRequestDto dto)
        {
            var result = await _authService.SignUpAsync(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequestDto dto)
        {
            var result = await _authService.SignInAsync(dto);
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Missing or invalid Authorization header");

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var user = await _authService.GetUserFromToken(token);

            if (user == null)
                return Unauthorized("Invalid or expired token");

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? ""
            };

            return Ok(userDto);
        }
    }
}
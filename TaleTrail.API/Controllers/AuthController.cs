using Microsoft.AspNetCore.Mvc;
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
    }
}
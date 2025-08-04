using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs.Auth.Signup;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public AuthController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email and password are required." });

            try
            {
                var session = await _supabase.Client.Auth.SignUp(request.Email, request.Password);

                if (session == null || session.User == null)
                    return StatusCode(500, new { message = "Signup failed. Supabase returned null user." });

                return Ok(new
                {
                    email = session.User.Email,
                    accessToken = session.AccessToken,
                    refreshToken = session.RefreshToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email and password are required." });

            try
            {
                var session = await _supabase.Client.Auth.SignIn(request.Email, request.Password);

                if (session == null || session.User == null)
                    return Unauthorized(new { message = "Invalid credentials or login failed." });

                return Ok(new
                {
                    email = session.User.Email,
                    accessToken = session.AccessToken,
                    refreshToken = session.RefreshToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _supabase.Client.Auth.SignOut();
                return Ok(new { message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }


}
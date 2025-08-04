using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public UserController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _supabase.Client.From<User>().Get();
            return Ok(ApiResponse<object>.SuccessResult(response.Models));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var response = await _supabase.Client
                .From<User>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var user = response.Models.FirstOrDefault();
            if (user == null)
                return NotFound(ApiResponse.ErrorResult("User not found"));

            return Ok(ApiResponse<object>.SuccessResult(user));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

            var existingUserResponse = await _supabase.Client
                .From<User>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var existingUser = existingUserResponse.Models.FirstOrDefault();
            if (existingUser == null)
                return NotFound(ApiResponse.ErrorResult("User not found"));

            existingUser.FullName = userDto.FullName;
            existingUser.Email = userDto.Email;
            existingUser.AvatarUrl = userDto.AvatarUrl;
            existingUser.Bio = userDto.Bio;

            var response = await _supabase.Client.From<User>().Update(existingUser);
            return Ok(ApiResponse<object>.SuccessResult(response.Models.First(), "User updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = new User { Id = id };
            await _supabase.Client.From<User>().Delete(user);
            return Ok(ApiResponse.SuccessResult("User deleted successfully"));
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile/my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var user = await GetCurrentUserAsync();

            var userResponse = new UserResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                Location = user.Location,
                // Role = user.Role, // <-- This line is now removed
                CreatedAt = user.CreatedAt
            };

            return Ok(ApiResponse<object>.SuccessResult(userResponse));
        }

        [HttpPut("profile/my-profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UserDto userDto)
        {
            var user = await GetCurrentUserAsync();
            var updatedUser = await _userService.UpdateUserAsync(user.Id, userDto);

            var userResponse = new UserResponseDTO
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                FullName = updatedUser.FullName,
                Bio = updatedUser.Bio,
                AvatarUrl = updatedUser.AvatarUrl,
                Location = updatedUser.Location,
                // Role = updatedUser.Role, // <-- This line is now removed
                CreatedAt = updatedUser.CreatedAt
            };

            return Ok(ApiResponse<object>.SuccessResult(userResponse));
        }

        [HttpDelete("profile/my-profile")]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var user = await GetCurrentUserAsync();
            await _userService.DeleteUserAsync(user.Id);
            return Ok(ApiResponse.SuccessResult("User deleted successfully"));
        }
    }
}
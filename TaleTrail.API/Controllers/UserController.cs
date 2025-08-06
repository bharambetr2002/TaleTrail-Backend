using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Add this for authorization
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;
using System;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize] // IMPORTANT: All actions in this controller require an authenticated user.
    public class UserController : BaseController
    {
        private readonly UserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the profile information for the currently authenticated user.
        /// </summary>
        [HttpGet("profile/my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound(ApiResponse.ErrorResult("User profile not found"));

                return Ok(ApiResponse<object>.SuccessResult(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user's profile");
                return BadRequest(ApiResponse.ErrorResult($"Error getting profile: {ex.Message}"));
            }
        }

        /// <summary>
        /// Updates the profile for the currently authenticated user.
        /// </summary>
        [HttpPut("profile/my-profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UserDto userDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var updatedUser = await _userService.UpdateUserAsync(userId, userDto);
                if (updatedUser == null)
                    return NotFound(ApiResponse.ErrorResult("User profile not found"));

                return Ok(ApiResponse<object>.SuccessResult(updatedUser, "Profile updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return BadRequest(ApiResponse.ErrorResult($"Error updating profile: {ex.Message}"));
            }
        }

        /// <summary>
        /// Deletes the profile for the currently authenticated user.
        /// </summary>
        [HttpDelete("profile/my-profile")]
        public async Task<IActionResult> DeleteMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _userService.DeleteUserAsync(userId);
                if (!success)
                    return NotFound(ApiResponse.ErrorResult("User profile not found"));

                // Note: You also need to handle deleting the user from Supabase Auth in the service.
                return Ok(ApiResponse.SuccessResult("User deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user profile");
                return BadRequest(ApiResponse.ErrorResult($"Error deleting user: {ex.Message}"));
            }
        }
    }
}
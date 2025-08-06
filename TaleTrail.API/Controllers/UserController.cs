using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<UserController> _logger;

        public UserController(SupabaseService supabase, ILogger<UserController> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var response = await _supabase.Client.From<User>().Get();
                var users = response.Models?.Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.AvatarUrl,
                    u.Bio,
                    u.CreatedAt
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(users, $"Found {users?.Count ?? 0} users"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return BadRequest(ApiResponse.ErrorResult($"Error getting users: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var response = await _supabase.Client
                    .From<User>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                var user = response.Models?.FirstOrDefault();
                if (user == null)
                    return NotFound(ApiResponse.ErrorResult("User not found"));

                var result = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.AvatarUrl,
                    user.Bio,
                    user.CreatedAt
                };

                return Ok(ApiResponse<object>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error getting user: {ex.Message}"));
            }
        }

        [HttpGet("profile/my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _supabase.Client
                    .From<User>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Get();

                var user = response.Models?.FirstOrDefault();
                if (user == null)
                    return NotFound(ApiResponse.ErrorResult("User profile not found"));

                var result = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.AvatarUrl,
                    user.Bio,
                    user.CreatedAt
                };

                return Ok(ApiResponse<object>.SuccessResult(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to get user profile");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return BadRequest(ApiResponse.ErrorResult($"Error getting profile: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var currentUserId = GetCurrentUserId();

                // Users can only update their own profile (or admin check could be added here)
                if (id != currentUserId)
                    return Forbid("You can only update your own profile");

                var existingUserResponse = await _supabase.Client
                    .From<User>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                var existingUser = existingUserResponse.Models?.FirstOrDefault();
                if (existingUser == null)
                    return NotFound(ApiResponse.ErrorResult("User not found"));

                existingUser.FullName = userDto.FullName;
                existingUser.Email = userDto.Email;
                existingUser.AvatarUrl = userDto.AvatarUrl;
                existingUser.Bio = userDto.Bio;

                var response = await _supabase.Client.From<User>().Update(existingUser);
                var updatedUser = response.Models?.FirstOrDefault();

                if (updatedUser == null)
                    return BadRequest(ApiResponse.ErrorResult("Failed to update user"));

                var result = new
                {
                    updatedUser.Id,
                    updatedUser.FullName,
                    updatedUser.Email,
                    updatedUser.AvatarUrl,
                    updatedUser.Bio,
                    updatedUser.CreatedAt
                };

                return Ok(ApiResponse<object>.SuccessResult(result, "User updated successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized user update attempt for user {UserId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error updating user: {ex.Message}"));
            }
        }

        [HttpPut("profile/my-profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var userId = GetCurrentUserId();

                var existingUserResponse = await _supabase.Client
                    .From<User>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Get();

                var existingUser = existingUserResponse.Models?.FirstOrDefault();
                if (existingUser == null)
                    return NotFound(ApiResponse.ErrorResult("User profile not found"));

                existingUser.FullName = userDto.FullName;
                existingUser.Email = userDto.Email;
                existingUser.AvatarUrl = userDto.AvatarUrl;
                existingUser.Bio = userDto.Bio;

                var response = await _supabase.Client.From<User>().Update(existingUser);
                var updatedUser = response.Models?.FirstOrDefault();

                if (updatedUser == null)
                    return BadRequest(ApiResponse.ErrorResult("Failed to update profile"));

                var result = new
                {
                    updatedUser.Id,
                    updatedUser.FullName,
                    updatedUser.Email,
                    updatedUser.AvatarUrl,
                    updatedUser.Bio,
                    updatedUser.CreatedAt
                };

                return Ok(ApiResponse<object>.SuccessResult(result, "Profile updated successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized profile update attempt");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return BadRequest(ApiResponse.ErrorResult($"Error updating profile: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Users can only delete their own profile (or admin check could be added here)
                if (id != currentUserId)
                    return Forbid("You can only delete your own profile");

                var user = new User { Id = id };
                await _supabase.Client.From<User>().Delete(user);

                _logger.LogInformation("User {UserId} deleted successfully", id);
                return Ok(ApiResponse.SuccessResult("User deleted successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized user deletion attempt for user {UserId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error deleting user: {ex.Message}"));
            }
        }
    }
}
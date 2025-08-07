using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : BaseController
{
    public UserController(UserService userService, ILogger<UserController> logger)
        : base(userService, logger) { }

    [HttpGet("profile")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var user = await GetCurrentUserAsync();
            var userDto = UserService.MapToUserResponseDTO(user);

            return Ok(ApiResponse<UserResponseDTO>.SuccessResponse("Profile retrieved successfully", userDto));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserResponseDTO>.ErrorResponse(ex.Message));
        }
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequestDTO request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var updatedUser = await _userService.UpdateUserAsync(userId, request);

            return Ok(ApiResponse<UserResponseDTO>.SuccessResponse("Profile updated successfully", updatedUser));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserResponseDTO>.ErrorResponse(ex.Message));
        }
    }
}
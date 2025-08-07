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
        var user = await GetCurrentUserAsync();
        var userDto = new UserResponseDTO
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName ?? "",
            Username = user.Username,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt
        };

        return Ok(ApiResponse<UserResponseDTO>.SuccessResponse("Profile retrieved successfully", userDto));
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequestDTO request)
    {
        var userId = GetCurrentUserId();
        var updatedUser = await _userService.UpdateUserAsync(userId, request);

        return Ok(ApiResponse<UserResponseDTO>.SuccessResponse("Profile updated successfully", updatedUser));
    }
}
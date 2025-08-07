using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseController
{
    public UserController(UserService userService) : base(userService) { }

    [HttpGet("profile/my-profile")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile()
    {
        var user = await GetCurrentUserAsync(); // Self-healing method
        return Ok(ApiResponse<User>.SuccessResponse("Fetched profile", user));
    }

    [HttpPut("profile/my-profile")]
    [Authorize]
    public async Task<IActionResult> UpdateMyProfile([FromBody] User updateRequest)
    {
        var userId = GetCurrentUserId();
        var updatedUser = await _userService.UpdateUserAsync(userId, updateRequest);
        return Ok(ApiResponse<User>.SuccessResponse("Profile updated", updatedUser));
    }

    [HttpDelete("profile/my-profile")]
    [Authorize]
    public async Task<IActionResult> DeleteMyProfile()
    {
        var userId = GetCurrentUserId();
        await _userService.DeleteUserAsync(userId);
        return Ok(ApiResponse<object>.SuccessResponse("Profile deleted", null));
    }
}
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly UserService _userService;

    public ProfileController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var user = await _userService.GetUserByUsernameAsync(username);
        if (user == null)
            return NotFound(ApiResponse<User>.ErrorResponse("User not found"));

        return Ok(ApiResponse<User>.SuccessResponse("Fetched public profile", user));
    }
}

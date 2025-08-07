using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model.DTOs;
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
        try
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound(ApiResponse<UserResponseDTO>.ErrorResponse("User not found"));

            var userDto = UserService.MapToUserResponseDTO(user);
            return Ok(ApiResponse<UserResponseDTO>.SuccessResponse("Public profile retrieved successfully", userDto));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserResponseDTO>.ErrorResponse(ex.Message));
        }
    }
}
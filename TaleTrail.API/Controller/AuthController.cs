using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignupRequest request)
    {
        try
        {
            var response = await _authService.SignUpAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse("User created successfully", response));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AuthResponse>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse("Login successful", response));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AuthResponse>.ErrorResponse(ex.Message));
        }
    }
}
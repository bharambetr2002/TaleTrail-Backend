using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaleTrail.API.Model;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

public abstract class BaseController : ControllerBase
{
    protected readonly UserService _userService;
    protected readonly ILogger _logger;

    protected BaseController(UserService userService, ILogger logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// The "self-healing" method that implements the core authentication flow.
    /// This method automatically creates user profiles on first authenticated request.
    /// </summary>
    protected async Task<User> GetCurrentUserAsync()
    {
        if (User?.Identity?.IsAuthenticated != true)
            throw new UnauthorizedAccessException("User is not authenticated");

        // Extract user ID from JWT "sub" claim
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID in token");

        // Check if user profile exists in our database
        var existingUser = await _userService.GetUserByIdAsync(userId);
        if (existingUser != null)
            return existingUser;

        // Self-healing: Create user profile from JWT claims
        var newUser = new User
        {
            Id = userId,
            Email = User.FindFirst("email")?.Value ??
                   User.FindFirst(ClaimTypes.Email)?.Value ?? "",
            FullName = User.FindFirst("full_name")?.Value ??
                      User.FindFirst(ClaimTypes.Name)?.Value ?? "",
            Username = User.FindFirst("username")?.Value ??
                      User.FindFirst("preferred_username")?.Value ??
                      User.FindFirst("email")?.Value?.Split('@')[0] ?? "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Create the user profile
        return await _userService.CreateUserAsync(newUser);
    }

    /// <summary>
    /// Helper method to get the current user's ID without creating a profile
    /// </summary>
    protected Guid GetCurrentUserId()
    {
        if (User?.Identity?.IsAuthenticated != true)
            throw new UnauthorizedAccessException("User is not authenticated");

        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID in token");

        return userId;
    }
}
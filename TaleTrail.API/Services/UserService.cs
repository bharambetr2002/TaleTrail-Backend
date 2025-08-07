using TaleTrail.API.Model;
using TaleTrail.API.DAO;
using System.Security.Claims;

namespace TaleTrail.API.Services;

public class UserService
{
    private readonly UserDao _userDao;
    private readonly ILogger<UserService> _logger;

    public UserService(UserDao userDao, ILogger<UserService> logger)
    {
        _userDao = userDao;
        _logger = logger;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userDao.GetByIdAsync(id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userDao.GetByUsernameAsync(username);
    }

    public async Task<User> CreateUserAsync(User newUser)
    {
        // Validate required fields
        if (string.IsNullOrEmpty(newUser.Email))
            throw new ArgumentException("Email is required");

        if (string.IsNullOrEmpty(newUser.Username))
            throw new ArgumentException("Username is required");

        // Set timestamps
        newUser.CreatedAt = DateTime.UtcNow;
        newUser.UpdatedAt = DateTime.UtcNow;

        try
        {
            return await _userDao.CreateAsync(newUser);
        }
        catch (Exception ex)
        {
            // Handle unique constraint violations (username already exists)
            if (ex.Message.Contains("duplicate") || ex.Message.Contains("unique"))
            {
                _logger.LogWarning("Username conflict for {Username}: {Error}", newUser.Username, ex.Message);
                throw new ArgumentException("Username already exists");
            }
            throw;
        }
    }

    public async Task<User> UpdateUserAsync(Guid userId, User updatedUser)
    {
        var existingUser = await _userDao.GetByIdAsync(userId);
        if (existingUser == null)
            throw new KeyNotFoundException("User not found");

        // Update allowed fields
        if (!string.IsNullOrEmpty(updatedUser.FullName))
            existingUser.FullName = updatedUser.FullName;

        if (!string.IsNullOrEmpty(updatedUser.Username) &&
            updatedUser.Username != existingUser.Username)
        {
            existingUser.Username = updatedUser.Username;
        }

        existingUser.Bio = updatedUser.Bio; // Allow null/empty for bio
        existingUser.AvatarUrl = updatedUser.AvatarUrl; // Allow null/empty for avatar

        try
        {
            return await _userDao.UpdateAsync(existingUser);
        }
        catch (Exception ex)
        {
            // Handle unique constraint violations
            if (ex.Message.Contains("duplicate") || ex.Message.Contains("unique"))
            {
                _logger.LogWarning("Username conflict during update for {Username}: {Error}", updatedUser.Username, ex.Message);
                throw new ArgumentException("Username already exists");
            }
            throw;
        }
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var existingUser = await _userDao.GetByIdAsync(userId);
        if (existingUser == null)
            throw new KeyNotFoundException("User not found");

        await _userDao.DeleteAsync(userId);
    }

    /// <summary>
    /// Thread-safe user creation from JWT claims with proper race condition handling
    /// </summary>
    public async Task<User> GetOrCreateUserFromClaimsAsync(ClaimsPrincipal claims)
    {
        // Extract and validate user ID
        var userIdClaim = claims.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID in token");

        // Check if user exists
        var user = await _userDao.GetByIdAsync(userId);
        if (user != null)
            return user;

        // Create user profile from JWT claims
        var email = claims.FindFirst("email")?.Value ?? claims.FindFirst(ClaimTypes.Email)?.Value ?? "";
        var fullName = claims.FindFirst("full_name")?.Value ?? claims.FindFirst(ClaimTypes.Name)?.Value ?? "";
        var username = claims.FindFirst("username")?.Value ??
                      claims.FindFirst("preferred_username")?.Value ??
                      $"user_{userId.ToString()[..8]}"; // Fallback to unique username

        var newUser = new User
        {
            Id = userId,
            Email = email,
            FullName = fullName,
            Username = username,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        try
        {
            return await _userDao.CreateAsync(newUser);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Race condition in user creation for {UserId}: {Error}", userId, ex.Message);

            // Race condition: Another request created the user, so fetch it
            var existingUser = await _userDao.GetByIdAsync(userId);
            if (existingUser != null)
                return existingUser;

            // If still null, something else went wrong
            throw new InvalidOperationException("Failed to create or retrieve user", ex);
        }
    }
}
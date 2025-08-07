using TaleTrail.API.Model;
using TaleTrail.API.DAO;
using System.Security.Claims;

namespace TaleTrail.API.Services;

public class UserService
{
    private readonly UserDao _userDao;

    public UserService(UserDao userDao)
    {
        _userDao = userDao;
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

        // Check if username already exists
        var existingUser = await _userDao.GetByUsernameAsync(newUser.Username);
        if (existingUser != null)
            throw new ArgumentException("Username already exists");

        // Set timestamps
        newUser.CreatedAt = DateTime.UtcNow;
        newUser.UpdatedAt = DateTime.UtcNow;

        return await _userDao.CreateAsync(newUser);
    }

    public async Task<User> UpdateUserAsync(Guid userId, User updatedUser)
    {
        var existingUser = await _userDao.GetByIdAsync(userId);
        if (existingUser == null)
            throw new Exception("User not found");

        // Check if new username conflicts with another user
        if (!string.IsNullOrEmpty(updatedUser.Username) &&
            updatedUser.Username != existingUser.Username)
        {
            var userWithSameUsername = await _userDao.GetByUsernameAsync(updatedUser.Username);
            if (userWithSameUsername != null && userWithSameUsername.Id != userId)
                throw new ArgumentException("Username already exists");
        }

        // Update allowed fields
        if (!string.IsNullOrEmpty(updatedUser.FullName))
            existingUser.FullName = updatedUser.FullName;

        if (!string.IsNullOrEmpty(updatedUser.Username))
            existingUser.Username = updatedUser.Username;

        existingUser.Bio = updatedUser.Bio; // Allow null/empty for bio
        existingUser.AvatarUrl = updatedUser.AvatarUrl; // Allow null/empty for avatar

        return await _userDao.UpdateAsync(existingUser);
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var existingUser = await _userDao.GetByIdAsync(userId);
        if (existingUser == null)
            throw new Exception("User not found");

        await _userDao.DeleteAsync(userId);
    }

    public async Task<User> GetOrCreateUserFromClaimsAsync(ClaimsPrincipal claims)
    {
        var userIdClaim = claims.FindFirst("sub")?.Value ?? claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new Exception("Invalid user ID in token");

        var user = await _userDao.GetByIdAsync(userId);
        if (user == null)
        {
            // Create user profile from JWT claims
            var email = claims.FindFirst("email")?.Value ?? claims.FindFirst(ClaimTypes.Email)?.Value ?? "";
            var fullName = claims.FindFirst("full_name")?.Value ?? claims.FindFirst(ClaimTypes.Name)?.Value ?? "";
            var username = claims.FindFirst("username")?.Value ??
                          claims.FindFirst("preferred_username")?.Value ??
                          email.Split('@')[0]; // Fallback to email prefix

            user = new User
            {
                Id = userId,
                Email = email,
                FullName = fullName,
                Username = username,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            user = await _userDao.CreateAsync(user);
        }

        return user;
    }
}
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

    public async Task<User> UpdateUserAsync(Guid userId, User updatedUser)
    {
        var existingUser = await _userDao.GetByIdAsync(userId);
        if (existingUser == null)
            throw new Exception("User not found");

        existingUser.FullName = updatedUser.FullName;
        existingUser.Username = updatedUser.Username;
        existingUser.Bio = updatedUser.Bio;
        existingUser.AvatarUrl = updatedUser.AvatarUrl;

        return await _userDao.UpdateAsync(existingUser);
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        await _userDao.DeleteAsync(userId);
    }

    public async Task<User> GetOrCreateUserFromClaimsAsync(ClaimsPrincipal claims)
    {
        var userIdClaim = claims.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new Exception("Invalid user ID in token");

        var user = await _userDao.GetByIdAsync(userId);
        if (user == null)
        {
            // Create user profile from JWT claims
            user = new User
            {
                Id = userId,
                Email = claims.FindFirst("email")?.Value ?? "",
                FullName = claims.FindFirst("full_name")?.Value ?? "",
                Username = claims.FindFirst("username")?.Value ?? "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            user = await _userDao.CreateAsync(user);
        }

        return user;
    }
}
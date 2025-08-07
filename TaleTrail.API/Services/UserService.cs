using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.DAO;

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

    public async Task<UserResponseDTO> UpdateUserAsync(Guid userId, UpdateUserRequestDTO request)
    {
        var user = await _userDao.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(request.FullName))
            user.FullName = request.FullName;

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            // Check if username is already taken
            var existingUser = await _userDao.GetByUsernameAsync(request.Username);
            if (existingUser != null && existingUser.Id != userId)
                throw new InvalidOperationException("Username is already taken");

            user.Username = request.Username;
        }

        user.Bio = request.Bio;
        user.AvatarUrl = request.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await _userDao.UpdateAsync(user);
        return MapToUserResponseDTO(updatedUser);
    }

    public async Task<User> CreateUserAsync(User newUser)
    {
        newUser.CreatedAt = DateTime.UtcNow;
        newUser.UpdatedAt = DateTime.UtcNow;
        return await _userDao.CreateAsync(newUser);
    }

    public static UserResponseDTO MapToUserResponseDTO(User user)
    {
        return new UserResponseDTO
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName ?? string.Empty,
            Username = user.Username,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
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

    public async Task<UserResponseDTO> UpdateUserAsync(Guid userId, UpdateUserRequestDTO request)
    {
        var user = await _userDao.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(request.FullName))
            user.FullName = request.FullName;

        if (!string.IsNullOrWhiteSpace(request.Username))
            user.Username = request.Username;

        user.Bio = request.Bio;
        user.AvatarUrl = request.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await _userDao.UpdateAsync(user);

        return new UserResponseDTO
        {
            Id = updatedUser.Id,
            Email = updatedUser.Email,
            FullName = updatedUser.FullName ?? "",
            Username = updatedUser.Username,
            Bio = updatedUser.Bio,
            AvatarUrl = updatedUser.AvatarUrl,
            CreatedAt = updatedUser.CreatedAt
        };
    }

    public async Task<User> CreateUserAsync(User newUser)
    {
        newUser.CreatedAt = DateTime.UtcNow;
        newUser.UpdatedAt = DateTime.UtcNow;
        return await _userDao.CreateAsync(newUser);
    }
}
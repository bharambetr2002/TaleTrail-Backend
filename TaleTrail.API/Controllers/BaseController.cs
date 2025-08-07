using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaleTrail.API.Services;
using TaleTrail.API.Models;
using System.Threading.Tasks;
using System.Text.Json;

namespace TaleTrail.API.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User not authenticated or User ID (sub) not found in token.");
        }

        protected async Task<User> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            var userService = HttpContext.RequestServices.GetRequiredService<UserService>();

            var user = await userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                return user;
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var userMetadataClaim = User.FindFirst("user_metadata")?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userMetadataClaim))
            {
                throw new Exception("Cannot create user profile. Essential claims (email, user_metadata) are missing from the JWT.");
            }

            using var jsonDoc = JsonDocument.Parse(userMetadataClaim);
            var fullName = jsonDoc.RootElement.GetProperty("full_name").GetString();
            var username = jsonDoc.RootElement.GetProperty("username").GetString();

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(username))
            {
                throw new Exception("Cannot create user profile. 'full_name' or 'username' is missing from the user_metadata in the JWT.");
            }

            var newUser = new User
            {
                Id = userId,
                Email = email,
                FullName = fullName,
                Username = username,
                CreatedAt = DateTime.UtcNow,
                // Role = "user" // <-- THIS IS THE LINE THAT CAUSED THE ERROR. IT IS NOW REMOVED.
            };

            var createdUser = await userService.CreateUserFromJwtAsync(newUser);
            if (createdUser == null)
            {
                throw new Exception("Failed to auto-create user profile in the database.");
            }

            return createdUser;
        }
    }
}
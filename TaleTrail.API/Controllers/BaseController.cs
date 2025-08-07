using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaleTrail.API.Services;
using TaleTrail.API.Models;
using System.Threading.Tasks;
using System.Text.Json; // Required for JSON parsing

namespace TaleTrail.API.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Gets the current user's ID from the token. Throws an error if not found.
        /// </summary>
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User not authenticated or User ID (sub) not found in token.");
        }

        /// <summary>
        /// A robust, self-healing method to get the current user.
        /// If the user exists in Supabase Auth but not in our public.users table,
        /// it will create them automatically.
        /// </summary>
        protected async Task<User> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            var userService = HttpContext.RequestServices.GetRequiredService<UserService>();

            // 1. Try to get the user from our database
            var user = await userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                return user; // User exists, return them immediately.
            }

            // 2. If user is null, they exist in Auth but not here. We must create them.
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            // --- THIS IS THE CORRECTED LOGIC ---
            // Supabase puts user metadata into a single claim called "user_metadata" which contains a JSON object.
            var userMetadataClaim = User.FindFirst("user_metadata")?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userMetadataClaim))
            {
                throw new Exception("Cannot create user profile. Essential claims (email, user_metadata) are missing from the JWT.");
            }

            // Parse the JSON from the claim
            using var jsonDoc = JsonDocument.Parse(userMetadataClaim);
            var fullName = jsonDoc.RootElement.GetProperty("full_name").GetString();
            var username = jsonDoc.RootElement.GetProperty("username").GetString();
            // ------------------------------------

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(username))
            {
                throw new Exception("Cannot create user profile. 'full_name' or 'username' is missing from the user_metadata in the JWT.");
            }

            var newUser = new User
            {
                Id = userId, // The correct ID from the token
                Email = email,
                FullName = fullName,
                Username = username,
                CreatedAt = DateTime.UtcNow,
                Role = "user"
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
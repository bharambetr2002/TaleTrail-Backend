// 🔧 COMPLETE BaseController.cs - Auto-sync users on each request
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using TaleTrail.API.Services;
using TaleTrail.API.Models;
using System.Threading.Tasks;
using TaleTrail.API.DAO;

namespace TaleTrail.API.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// ✅ UPDATED: Gets current user with automatic sync if missing
        /// This ensures ANY authenticated request creates the user if they don't exist
        /// </summary>
        protected async Task<User> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            var userService = HttpContext.RequestServices.GetRequiredService<UserService>();
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<BaseController>>();

            // 1. Try to get user from database
            var user = await userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                return user; // ✅ User exists, return immediately
            }

            // 2. ✅ CRITICAL FIX: User missing from database, but JWT is valid
            // This means Supabase Auth user exists but our database record doesn't
            logger.LogWarning("🔧 User {UserId} authenticated but missing from database. Auto-creating...", userId);

            try
            {
                // Extract user info from JWT claims
                var email = GetCurrentUserEmail();

                // Create user profile from JWT claims
                user = await CreateUserFromJwtClaims(userId, email);

                if (user != null)
                {
                    logger.LogInformation("✅ Successfully auto-created user: {UserId} ({Username})", user.Id, user.Username);
                    return user;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Failed to auto-create user {UserId} from JWT", userId);
            }

            // 3. If all else fails, throw clear error
            throw new UnauthorizedAccessException("User profile not found and could not be auto-created. Please re-login or contact support.");
        }

        /// <summary>
        /// Creates user profile from JWT claims when database record is missing
        /// </summary>
        private async Task<User?> CreateUserFromJwtClaims(Guid userId, string email)
        {
            var userDao = HttpContext.RequestServices.GetRequiredService<UserDao>();
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<BaseController>>();

            try
            {
                // Generate username from email
                var username = await GenerateUsernameFromEmail(email);

                var newUser = new User
                {
                    Id = userId, // ✅ Use Supabase ID from JWT
                    Email = email.ToLowerInvariant(),
                    FullName = ExtractFullNameFromEmail(email),
                    Username = username,
                    CreatedAt = DateTime.UtcNow,
                    Role = "user"
                };

                var createdUser = await userDao.AddAsync(newUser);

                if (createdUser != null)
                {
                    logger.LogInformation("✅ Auto-created user from JWT: {UserId} ({Username})", createdUser.Id, createdUser.Username);
                }

                return createdUser;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Failed to create user from JWT claims");

                // Handle race conditions - another request may have created the user
                if (ex.Message.Contains("duplicate key") || ex.Message.Contains("23505"))
                {
                    logger.LogInformation("🔄 Race condition detected, attempting to retrieve existing user");
                    try
                    {
                        var existingUser = await userDao.GetByIdAsync(userId);
                        if (existingUser != null)
                        {
                            logger.LogInformation("✅ Found user created by concurrent request: {UserId}", userId);
                            return existingUser;
                        }
                    }
                    catch (Exception retrieveEx)
                    {
                        logger.LogError(retrieveEx, "❌ Failed to retrieve user after race condition");
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Generates unique username from email
        /// </summary>
        private async Task<string> GenerateUsernameFromEmail(string email)
        {
            var userDao = HttpContext.RequestServices.GetRequiredService<UserDao>();

            var baseUsername = CleanUsername(email.Split('@')[0]);
            var counter = 1;
            var candidateUsername = baseUsername;

            while (await userDao.UsernameExistsAsync(candidateUsername))
            {
                candidateUsername = $"{baseUsername}_{counter}";
                counter++;

                if (counter > 1000)
                {
                    candidateUsername = $"user_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                    break;
                }
            }

            return candidateUsername;
        }

        /// <summary>
        /// Cleans username format
        /// </summary>
        private string CleanUsername(string input)
        {
            if (string.IsNullOrEmpty(input)) return "user";

            var cleaned = System.Text.RegularExpressions.Regex.Replace(input.ToLowerInvariant(), @"[^a-z0-9_]", "");

            if (cleaned.Length == 0 || (!char.IsLetter(cleaned[0]) && cleaned[0] != '_'))
                cleaned = "u" + cleaned;

            if (cleaned.Length < 3)
                cleaned = cleaned.PadRight(3, '1');

            if (cleaned.Length > 50)
                cleaned = cleaned.Substring(0, 50);

            return cleaned;
        }

        /// <summary>
        /// Extracts a reasonable full name from email
        /// </summary>
        private string ExtractFullNameFromEmail(string email)
        {
            try
            {
                var localPart = email.Split('@')[0];
                // Convert dots/underscores to spaces and capitalize
                var name = localPart.Replace(".", " ").Replace("_", " ");
                return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
            }
            catch
            {
                return "User";
            }
        }

        // ✅ Existing helper methods remain the same
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("User not authenticated or User ID not found in token.");
        }

        protected string GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            if (roleClaim != null && !string.IsNullOrEmpty(roleClaim.Value))
            {
                return roleClaim.Value;
            }

            throw new UnauthorizedAccessException("Role not found in token.");
        }

        protected string GetCurrentUserEmail()
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
            {
                return emailClaim.Value;
            }

            throw new UnauthorizedAccessException("Email not found in token.");
        }

        protected async Task<User> ValidateCurrentUserAsync(string? requiredRole = null)
        {
            var user = await GetCurrentUserAsync();

            if (!string.IsNullOrEmpty(requiredRole) && !user.Role.Equals(requiredRole, StringComparison.OrdinalIgnoreCase))
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<BaseController>>();
                logger.LogWarning("🚨 User {UserId} attempted to access {RequiredRole} endpoint with role {UserRole}",
                    user.Id, requiredRole, user.Role);

                throw new UnauthorizedAccessException($"Access denied. {requiredRole} role required.");
            }

            return user;
        }

        protected bool IsCurrentUserAdmin()
        {
            try
            {
                var role = GetCurrentUserRole();
                return role.Equals("admin", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        protected void LogUserAction(string action, object? details = null)
        {
            try
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<BaseController>>();
                var userId = GetCurrentUserId();
                var userEmail = GetCurrentUserEmail();

                logger.LogInformation("👤 User Action: {UserId} ({Email}) performed {Action} | Details: {Details}",
                    userId, userEmail, action, details ?? "None");
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<BaseController>>();
                logger.LogWarning("⚠️ Failed to log user action: {Error}", ex.Message);
            }
        }

        protected string GetUserInfoForLogging()
        {
            try
            {
                var userId = GetCurrentUserId();
                var email = GetCurrentUserEmail();
                return $"User: {userId} ({email})";
            }
            catch
            {
                return "User: [Anonymous/Invalid Token]";
            }
        }
    } // ✅ End of BaseController class
} // ✅ End of namespace
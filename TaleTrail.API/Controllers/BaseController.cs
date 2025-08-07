using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using TaleTrail.API.Services;
using TaleTrail.API.Models;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Gets the authenticated user's ID from the JWT claims.
        /// </summary>
        /// <returns>The user's GUID.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if the user ID claim is not found.</exception>
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("User not authenticated or User ID not found in token.");
        }

        /// <summary>
        /// Gets the authenticated user's role from the JWT claims.
        /// </summary>
        /// <returns>The user's role string (e.g., "user" or "admin").</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if the role claim is not found.</exception>
        protected string GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            if (roleClaim != null && !string.IsNullOrEmpty(roleClaim.Value))
            {
                return roleClaim.Value;
            }

            throw new UnauthorizedAccessException("Role not found in token.");
        }

        /// <summary>
        /// Gets the authenticated user's email from the JWT claims.
        /// </summary>
        /// <returns>The user's email.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if the email claim is not found.</exception>
        protected string GetCurrentUserEmail()
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
            {
                return emailClaim.Value;
            }

            throw new UnauthorizedAccessException("Email not found in token.");
        }

        /// <summary>
        /// Gets the current user's full profile, ensuring they exist in the database.
        /// This method validates that the JWT user actually exists in your users table.
        /// </summary>
        /// <returns>The user's profile from the database.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if user doesn't exist in database.</exception>
        protected async Task<User> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            var userService = HttpContext.RequestServices.GetRequiredService<UserService>();

            var user = await userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<BaseController>>();
                logger.LogWarning("🚨 JWT token valid but user {UserId} not found in database. This suggests a sync issue.", userId);

                throw new UnauthorizedAccessException("User profile not found. Please complete your account setup or contact support.");
            }

            return user;
        }

        /// <summary>
        /// Validates that the current user exists and optionally checks their role.
        /// Use this in controllers that need to ensure database user existence.
        /// </summary>
        /// <param name="requiredRole">Optional role requirement (e.g., "admin")</param>
        /// <returns>The validated user profile</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if user doesn't exist or lacks required role.</exception>
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

        /// <summary>
        /// Helper method to check if the current user is an admin without throwing exceptions.
        /// </summary>
        /// <returns>True if user is admin, false otherwise</returns>
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

        /// <summary>
        /// Helper method to log user actions for debugging and security auditing.
        /// </summary>
        /// <param name="action">The action being performed</param>
        /// <param name="details">Optional additional details</param>
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
                // Don't let logging failures break the request
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<BaseController>>();
                logger.LogWarning("⚠️ Failed to log user action: {Error}", ex.Message);
            }
        }

        /// <summary>
        /// Helper method to get user info for debugging without throwing exceptions.
        /// </summary>
        /// <returns>Safe user info for logging</returns>
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
    }
}
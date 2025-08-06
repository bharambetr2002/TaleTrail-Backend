using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims; // Required for accessing claims

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
            // The User.FindFirst method is the standard .NET way to get claims from the validated JWT.
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
    }
}
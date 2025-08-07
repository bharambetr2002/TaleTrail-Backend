using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaleTrail.API.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// A simple helper to get the current user's ID from their token.
        /// </summary>
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }

            // This will trigger if an endpoint marked [Authorize] is hit without a valid token.
            throw new UnauthorizedAccessException("User ID could not be found in the token.");
        }
    }
}
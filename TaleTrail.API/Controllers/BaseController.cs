using Microsoft.AspNetCore.Mvc;

namespace TaleTrail.API.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected Guid GetCurrentUserId()
        {
            if (HttpContext.Items["UserId"] is Guid userId)
            {
                return userId;
            }

            throw new UnauthorizedAccessException("User not authenticated or user ID not found in context");
        }

        protected string? GetCurrentUserEmail()
        {
            return HttpContext.Items["UserEmail"] as string;
        }

        protected string? GetCurrentUserToken()
        {
            return HttpContext.Items["AccessToken"] as string;
        }

        protected bool IsUserAuthenticated()
        {
            return HttpContext.Items.ContainsKey("UserId") && HttpContext.Items["UserId"] is Guid;
        }
    }
}
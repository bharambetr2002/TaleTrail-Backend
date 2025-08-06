using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TaleTrail.API.Services;
using System.Linq;

namespace TaleTrail.API.Middleware
{
    public class SupabaseAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SupabaseAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, JwtService jwtService)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            // If a token exists and user isn't already authenticated, validate it and create a user principal
            if (!string.IsNullOrEmpty(authHeader) &&
                authHeader.StartsWith("Bearer ") &&
                !context.User.Identity?.IsAuthenticated == true)
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                try
                {
                    // Use the JwtService to get the claims from the token
                    var claims = jwtService.GetClaimsFromToken(token);

                    // Create the identity and principal that .NET's [Authorize] attribute will use
                    var identity = new ClaimsIdentity(claims, "supabase-jwt");
                    context.User = new ClaimsPrincipal(identity);
                }
                catch
                {
                    // If token is invalid, do nothing. The user remains anonymous.
                    // The [Authorize] attribute on the controller will handle the rejection.
                }
            }

            await _next(context);
        }
    }
}
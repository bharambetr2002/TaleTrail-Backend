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
        private readonly ILogger<SupabaseAuthMiddleware> _logger;

        public SupabaseAuthMiddleware(RequestDelegate next, ILogger<SupabaseAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, JwtService jwtService)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            // Only process if we have a Bearer token and user isn't already authenticated
            if (!string.IsNullOrEmpty(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) &&
                !context.User.Identity?.IsAuthenticated == true)
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                try
                {
                    _logger.LogDebug("🔍 Processing Supabase JWT token for request: {Path}", context.Request.Path);

                    // Use the JwtService to get the claims from the token
                    var claims = jwtService.GetClaimsFromToken(token);
                    var claimsList = claims.ToList();

                    if (claimsList.Any())
                    {
                        // Create the identity and principal that .NET's [Authorize] attribute will use
                        var identity = new ClaimsIdentity(claimsList, "supabase-jwt");
                        context.User = new ClaimsPrincipal(identity);

                        var userId = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                        var email = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                        var role = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                        _logger.LogDebug("✅ Successfully authenticated user: {UserId} ({Email}) with role: {Role}",
                            userId, email, role);

                        // Optional: Add user validation here
                        // This would check if the JWT user exists in your database
                        // Uncomment if you want strict user validation:
                        /*
                        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
                        {
                            var userService = context.RequestServices.GetRequiredService<UserService>();
                            var user = await userService.GetUserByIdAsync(userGuid);
                            
                            if (user == null)
                            {
                                _logger.LogWarning("🚨 JWT token valid but user {UserId} not found in database", userId);
                                // You could either:
                                // 1. Allow request to continue (current behavior)
                                // 2. Clear authentication and return 401
                                // 3. Redirect to account completion
                                
                                // Option 2 - Strict validation:
                                // context.User = new ClaimsPrincipal();
                                // context.Response.StatusCode = 401;
                                // await context.Response.WriteAsync("User profile not found");
                                // return;
                            }
                        }
                        */
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ JWT token processed but no claims extracted");
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    _logger.LogWarning("🚫 JWT token validation failed: {Error}", ex.Message);
                    // Token is invalid, user remains anonymous
                    // The [Authorize] attribute on controllers will handle the rejection
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 Unexpected error during JWT token processing");
                    // Token processing failed, user remains anonymous
                }
            }
            else if (!string.IsNullOrEmpty(authHeader))
            {
                _logger.LogDebug("🔍 Auth header present but not Bearer token or user already authenticated");
            }

            await _next(context);
        }
    }
}
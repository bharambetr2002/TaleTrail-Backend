using System.Text.Json;
using Supabase.Gotrue;

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

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip auth for public endpoints
            var path = context.Request.Path.Value?.ToLower();
            var publicPaths = new[] { "/api/auth/login", "/api/auth/signup", "/swagger", "/health" };

            if (publicPaths.Any(p => path?.StartsWith(p) == true))
            {
                await _next(context);
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader?.StartsWith("Bearer ") == true)
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                try
                {
                    // Add user info to context for use in controllers
                    context.Items["UserId"] = ExtractUserIdFromToken(token);
                    context.Items["AccessToken"] = token;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Invalid token provided");
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        Success = false,
                        Message = "Invalid or expired token"
                    }));
                    return;
                }
            }

            await _next(context);
        }

        private string? ExtractUserIdFromToken(string token)
        {
            // This would typically decode the JWT token
            // For now, return null to indicate token validation is needed
            return null;
        }
    }
}
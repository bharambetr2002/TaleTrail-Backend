using System.Security.Claims;
using TaleTrail.API.Services;

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

            // Only process if we have a Bearer token
            if (!string.IsNullOrEmpty(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) &&
                !context.User.Identity?.IsAuthenticated == true)
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                try
                {
                    _logger.LogDebug("Processing JWT token for: {Path}", context.Request.Path);

                    var claims = jwtService.GetClaimsFromToken(token);
                    var claimsList = claims.ToList();

                    if (claimsList.Any())
                    {
                        var identity = new ClaimsIdentity(claimsList, "supabase-jwt");
                        context.User = new ClaimsPrincipal(identity);

                        var userId = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                        var email = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                        _logger.LogDebug("Authenticated user: {UserId} ({Email})", userId, email);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    _logger.LogWarning("JWT validation failed: {Error}", ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing JWT token");
                }
            }

            await _next(context);
        }
    }
}
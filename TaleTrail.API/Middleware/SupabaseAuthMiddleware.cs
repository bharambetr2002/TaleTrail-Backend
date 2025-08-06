using System.Text.Json;
using TaleTrail.API.Services;

namespace TaleTrail.API.Middleware
{
    public class SupabaseAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SupabaseAuthMiddleware> _logger;
        private readonly JwtService _jwtService;

        public SupabaseAuthMiddleware(RequestDelegate next, ILogger<SupabaseAuthMiddleware> logger, JwtService jwtService)
        {
            _next = next;
            _logger = logger;
            _jwtService = jwtService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip auth for public endpoints
            var path = context.Request.Path.Value?.ToLower();
            var publicPaths = new[] {
                "/api/auth/login",
                "/api/auth/signup",
                "/swagger",
                "/health",
                "/",
                "/api/author", // GET only for public viewing
                "/api/category", // GET only for public viewing
                "/api/publisher" // GET only for public viewing
            };

            // Allow GET requests to certain endpoints without auth
            var isGetRequest = context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase);
            var publicGetPaths = new[] { "/api/book", "/api/blog", "/api/review" };

            if (publicPaths.Any(p => path?.StartsWith(p) == true) ||
                (isGetRequest && publicGetPaths.Any(p => path?.StartsWith(p) == true)))
            {
                await _next(context);
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                await WriteUnauthorizedResponse(context, "Missing or invalid Authorization header");
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var userId = _jwtService.GetUserIdFromToken(token);
                var userEmail = _jwtService.GetUserEmailFromToken(token);

                // Add user info to context for use in controllers
                context.Items["UserId"] = userId;
                context.Items["UserEmail"] = userEmail;
                context.Items["AccessToken"] = token;

                _logger.LogDebug("User {UserId} authenticated successfully", userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteUnauthorizedResponse(context, ex.Message);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication error");
                await WriteUnauthorizedResponse(context, "Authentication failed");
                return;
            }

            await _next(context);
        }

        private async Task WriteUnauthorizedResponse(HttpContext context, string message)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var response = JsonSerializer.Serialize(new
            {
                success = false,
                message = message,
                statusCode = 401,
                timestamp = DateTime.UtcNow
            }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(response);
        }
    }
}
using System.Net;
using System.Text.Json;
using TaleTrail.API.Exceptions;

namespace TaleTrail.API.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                var (statusCode, message, details) = GetErrorResponse(error);

                LogError(error, context, statusCode, message);

                response.StatusCode = (int)statusCode;

                var jsonResponse = JsonSerializer.Serialize(new
                {
                    success = false,
                    message = message,
                    statusCode = (int)statusCode,
                    timestamp = DateTime.UtcNow,
                    path = context.Request.Path.Value,
                    method = context.Request.Method,
                    details = details,
                    traceId = context.TraceIdentifier
                }, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await response.WriteAsync(jsonResponse);
            }
        }

        private (HttpStatusCode statusCode, string message, object? details) GetErrorResponse(Exception error)
        {
            return error switch
            {
                // Auth Errors
                UnauthorizedAccessException ex => (
                    HttpStatusCode.Unauthorized,
                    ex.Message,
                    new { type = "AuthenticationError" }
                ),

                // Validation
                ValidationException ex => (
                    HttpStatusCode.BadRequest,
                    ex.Message,
                    new { type = "ValidationError", errors = ex.Errors }
                ),

                System.ComponentModel.DataAnnotations.ValidationException ex => (
                    HttpStatusCode.BadRequest,
                    ex.Message,
                    new { type = "ModelValidationError" }
                ),

                // Not Found
                NotFoundException ex => (
                    HttpStatusCode.NotFound,
                    ex.Message,
                    new { type = "NotFoundError" }
                ),

                // Business Logic
                AppException ex => (
                    HttpStatusCode.BadRequest,
                    ex.Message,
                    new { type = "BusinessLogicError" }
                ),

                // Supabase errors
                Supabase.Gotrue.Exceptions.GotrueException ex => (
                    HttpStatusCode.BadRequest,
                    $"Authentication error: {ex.Message}",
                    new { type = "SupabaseAuthError", reason = ex.Reason }
                ),

                Supabase.Postgrest.Exceptions.PostgrestException ex => (
                    HttpStatusCode.InternalServerError,
                    "Database operation failed",
                    new { type = "DatabaseError", details = ex.Message }
                ),

                // Rate Limiting (fallback for .NET 8)
                Exception ex when ex.Message.Contains("RateLimiter") => (
                    HttpStatusCode.TooManyRequests,
                    "Too many requests. Please try again later.",
                    new { type = "RateLimitError" }
                ),

                // Foreign Key
                Exception ex when ex.Message.Contains("foreign key constraint") => (
                    HttpStatusCode.BadRequest,
                    "Invalid reference to related data",
                    new { type = "ForeignKeyError", hint = "The referenced item may not exist" }
                ),

                // Duplicate
                Exception ex when ex.Message.Contains("duplicate key") || ex.Message.Contains("23505") => (
                    HttpStatusCode.Conflict,
                    "This item already exists",
                    new { type = "DuplicateError" }
                ),

                // Timeout
                TaskCanceledException ex when ex.InnerException is TimeoutException => (
                    HttpStatusCode.RequestTimeout,
                    "The request timed out",
                    new { type = "TimeoutError" }
                ),

                TimeoutException ex => (
                    HttpStatusCode.RequestTimeout,
                    "Operation timed out",
                    new { type = "TimeoutError" }
                ),

                JsonException ex => (
                    HttpStatusCode.BadRequest,
                    "Invalid JSON format",
                    new { type = "JsonParsingError" }
                ),

                // MUST come before ArgumentException
                ArgumentNullException ex => (
                    HttpStatusCode.BadRequest,
                    $"Required parameter is missing: {ex.ParamName}",
                    new { type = "MissingParameterError", parameter = ex.ParamName }
                ),

                ArgumentException ex => (
                    HttpStatusCode.BadRequest,
                    ex.Message,
                    new { type = "ArgumentError" }
                ),

                // Fallback
                _ => (
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred",
                    new { type = "InternalServerError" }
                )
            };
        }

        private void LogError(Exception error, HttpContext context, HttpStatusCode statusCode, string message)
        {
            var userInfo = GetUserInfoFromContext(context);
            var requestInfo = $"{context.Request.Method} {context.Request.Path}";

            if (statusCode >= HttpStatusCode.InternalServerError)
            {
                _logger.LogError(error,
                    "💥 SERVER ERROR: {StatusCode} | {RequestInfo} | User: {UserInfo} | Message: {Message}",
                    (int)statusCode, requestInfo, userInfo, message);
            }
            else if (statusCode == HttpStatusCode.Unauthorized || statusCode == HttpStatusCode.Forbidden)
            {
                _logger.LogWarning(
                    "🚫 AUTH ERROR: {StatusCode} | {RequestInfo} | User: {UserInfo} | Message: {Message}",
                    (int)statusCode, requestInfo, userInfo, message);
            }
            else if (statusCode >= HttpStatusCode.BadRequest)
            {
                _logger.LogInformation(
                    "⚠️ CLIENT ERROR: {StatusCode} | {RequestInfo} | User: {UserInfo} | Message: {Message}",
                    (int)statusCode, requestInfo, userInfo, message);
            }
        }

        private string GetUserInfoFromContext(HttpContext context)
        {
            try
            {
                var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var email = context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(email))
                    return $"{userId} ({email})";

                if (!string.IsNullOrEmpty(userId))
                    return userId;

                return "Anonymous";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}

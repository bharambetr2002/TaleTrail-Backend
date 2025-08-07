using System.Net;
using System.Text.Json;

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

                var (statusCode, message) = GetErrorResponse(error);

                response.StatusCode = (int)statusCode;

                _logger.LogError(error, "API Error: {StatusCode} - {Message}", (int)statusCode, message);

                var jsonResponse = JsonSerializer.Serialize(new
                {
                    success = false,
                    message = message,
                    statusCode = (int)statusCode,
                    timestamp = DateTime.UtcNow,
                    path = context.Request.Path.Value
                }, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await response.WriteAsync(jsonResponse);
            }
        }

        private (HttpStatusCode statusCode, string message) GetErrorResponse(Exception error)
        {
            return error switch
            {
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, error.Message),
                ArgumentException => (HttpStatusCode.BadRequest, error.Message),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
                TimeoutException => (HttpStatusCode.RequestTimeout, "Request timed out"),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
            };
        }
    }
}
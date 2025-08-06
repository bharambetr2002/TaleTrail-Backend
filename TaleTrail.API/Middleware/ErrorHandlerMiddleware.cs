// TaleTrail.API/Middleware/ErrorHandlerMiddleware.cs
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

                var (statusCode, message) = error switch
                {
                    ValidationException ex => (HttpStatusCode.BadRequest, ex.Message),
                    NotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
                    AppException ex => (HttpStatusCode.BadRequest, ex.Message),
                    Supabase.Gotrue.Exceptions.GotrueException ex => (HttpStatusCode.BadRequest, $"Authentication error: {ex.Message}"),
                    Supabase.Postgrest.Exceptions.PostgrestException ex => (HttpStatusCode.InternalServerError, $"Database error: {ex.Message}"),
                    _ => (HttpStatusCode.InternalServerError, "An internal server error occurred")
                };

                // Log detailed error for debugging
                _logger.LogError(error, "Error occurred: {StatusCode} - {Message}", statusCode, message);

                response.StatusCode = (int)statusCode;

                var jsonResponse = JsonSerializer.Serialize(new
                {
                    success = false,
                    message = message,
                    statusCode = (int)statusCode,
                    timestamp = DateTime.UtcNow
                }, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await response.WriteAsync(jsonResponse);
            }
        }
    }
}
using System.Net;
using System.Text.Json;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

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
                _logger.LogError(error, "An unhandled exception occurred");

                var response = context.Response;
                response.ContentType = "application/json";

                var apiResponse = error switch
                {
                    ValidationException ex => new ApiErrorResponse
                    {
                        Success = false,
                        Message = ex.Message,
                        Errors = ex.Errors,
                        StatusCode = (int)HttpStatusCode.BadRequest
                    },
                    NotFoundException ex => new ApiErrorResponse
                    {
                        Success = false,
                        Message = ex.Message,
                        StatusCode = (int)HttpStatusCode.NotFound
                    },
                    AppException ex => new ApiErrorResponse
                    {
                        Success = false,
                        Message = ex.Message,
                        StatusCode = (int)HttpStatusCode.BadRequest
                    },
                    _ => new ApiErrorResponse
                    {
                        Success = false,
                        Message = "An internal server error occurred",
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    }
                };

                response.StatusCode = apiResponse.StatusCode;

                var jsonResponse = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await response.WriteAsync(jsonResponse);
            }
        }

        private class ApiErrorResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Errors { get; set; }
            public int StatusCode { get; set; }
        }
    }
}
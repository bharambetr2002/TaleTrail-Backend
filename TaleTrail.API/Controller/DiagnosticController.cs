using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Services;
using TaleTrail.API.Model;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiagnosticController : ControllerBase
{
    private readonly SupabaseService _supabaseService;
    private readonly ILogger<DiagnosticController> _logger;

    public DiagnosticController(SupabaseService supabaseService, ILogger<DiagnosticController> logger)
    {
        _supabaseService = supabaseService;
        _logger = logger;
    }

    [HttpGet("connection")]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            _logger.LogInformation("Testing Supabase connection...");

            // Test basic connection
            var response = await _supabaseService.Supabase
                .From<Author>()
                .Limit(1)
                .Get();

            var connectionInfo = new
            {
                IsConnected = response != null,
                ResponseReceived = response != null,
                ModelsCount = response?.Models?.Count ?? 0,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Connection test result: {@ConnectionInfo}", connectionInfo);

            return Ok(ApiResponse<object>.SuccessResponse("Connection test completed", connectionInfo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection test failed");
            return StatusCode(500, ApiResponse<object>.ErrorResponse($"Connection failed: {ex.Message}"));
        }
    }

    [HttpGet("tables")]
    public async Task<IActionResult> TestTables()
    {
        var tableTests = new Dictionary<string, object>();

        try
        {
            // Test Authors table
            try
            {
                var authorsResponse = await _supabaseService.Supabase.From<Author>().Limit(1).Get();
                tableTests["authors"] = new
                {
                    Success = true,
                    Count = authorsResponse?.Models?.Count ?? 0,
                    HasData = authorsResponse?.Models?.Any() == true
                };
            }
            catch (Exception ex)
            {
                tableTests["authors"] = new { Success = false, Error = ex.Message };
            }

            // Test Books table
            try
            {
                var booksResponse = await _supabaseService.Supabase.From<Book>().Limit(1).Get();
                tableTests["books"] = new
                {
                    Success = true,
                    Count = booksResponse?.Models?.Count ?? 0,
                    HasData = booksResponse?.Models?.Any() == true
                };
            }
            catch (Exception ex)
            {
                tableTests["books"] = new { Success = false, Error = ex.Message };
            }

            // Test Publishers table
            try
            {
                var publishersResponse = await _supabaseService.Supabase.From<Publisher>().Limit(1).Get();
                tableTests["publishers"] = new
                {
                    Success = true,
                    Count = publishersResponse?.Models?.Count ?? 0,
                    HasData = publishersResponse?.Models?.Any() == true
                };
            }
            catch (Exception ex)
            {
                tableTests["publishers"] = new { Success = false, Error = ex.Message };
            }

            return Ok(ApiResponse<object>.SuccessResponse("Table tests completed", tableTests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Table tests failed");
            return StatusCode(500, ApiResponse<object>.ErrorResponse($"Table tests failed: {ex.Message}"));
        }
    }

    [HttpGet("environment")]
    public IActionResult CheckEnvironment()
    {
        try
        {
            var envInfo = new
            {
                HasSupabaseUrl = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SUPABASE_URL")),
                HasSupabaseKey = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SUPABASE_KEY")),
                HasJwtSecret = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET")),
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                SupabaseUrlLength = Environment.GetEnvironmentVariable("SUPABASE_URL")?.Length ?? 0,
                SupabaseKeyLength = Environment.GetEnvironmentVariable("SUPABASE_KEY")?.Length ?? 0
            };

            return Ok(ApiResponse<object>.SuccessResponse("Environment check completed", envInfo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Environment check failed");
            return StatusCode(500, ApiResponse<object>.ErrorResponse($"Environment check failed: {ex.Message}"));
        }
    }
}
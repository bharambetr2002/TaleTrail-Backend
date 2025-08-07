using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;
using TaleTrail.API.DAO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    /// <summary>
    /// Diagnostic controller for testing authentication, database connectivity, and system health.
    /// Remove this controller in production or secure it with admin-only access.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticController : BaseController
    {
        private readonly JwtService _jwtService;
        private readonly UserService _userService;
        private readonly UserDao _userDao;
        private readonly SupabaseService _supabaseService;
        private readonly ILogger<DiagnosticController> _logger;

        public DiagnosticController(
            JwtService jwtService,
            UserService userService,
            UserDao userDao,
            SupabaseService supabaseService,
            ILogger<DiagnosticController> logger)
        {
            _jwtService = jwtService;
            _userService = userService;
            _userDao = userDao;
            _supabaseService = supabaseService;
            _logger = logger;
        }

        /// <summary>
        /// Test endpoint to validate JWT token format and extraction
        /// </summary>
        [HttpPost("validate-token")]
        public IActionResult ValidateToken([FromBody] TokenTestRequest request)
        {
            try
            {
                _logger.LogInformation("🔍 Testing token validation for token: {TokenPreview}",
                    request.Token?.Length > 20 ? request.Token[..20] + "..." : request.Token);

                if (string.IsNullOrEmpty(request.Token))
                {
                    return BadRequest(ApiResponse.ErrorResult("Token is required"));
                }

                var isValidFormat = _jwtService.IsValidJwtFormat(request.Token);
                if (!isValidFormat)
                {
                    return BadRequest(ApiResponse.ErrorResult("Token format is invalid (should be JWT with 3 parts separated by dots)"));
                }

                var claims = _jwtService.GetClaimsFromToken(request.Token);
                var claimsInfo = claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(new
                {
                    isValid = true,
                    claims = claimsInfo,
                    message = "Token is valid and claims extracted successfully"
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Token validation failed");
                return BadRequest(ApiResponse.ErrorResult($"Token validation failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// Test endpoint that requires authentication - tests the full auth pipeline
        /// </summary>
        [HttpGet("auth-test")]
        [Authorize]
        public async Task<IActionResult> AuthTest()
        {
            try
            {
                var userId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();
                var userEmail = GetCurrentUserEmail();

                // Test if user exists in database
                var user = await _userService.GetUserByIdAsync(userId);

                return Ok(ApiResponse<object>.SuccessResult(new
                {
                    authentication = "✅ SUCCESS",
                    userId = userId,
                    email = userEmail,
                    role = userRole,
                    userExistsInDatabase = user != null,
                    userProfile = user != null ? new
                    {
                        user.Username,
                        user.FullName,
                        user.CreatedAt
                    } : null,
                    message = user != null
                        ? "Authentication successful and user found in database"
                        : "⚠️ Authentication successful but user NOT found in database (potential sync issue)"
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Auth test failed");
                return BadRequest(ApiResponse.ErrorResult($"Authentication test failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// Test database connectivity and user synchronization
        /// </summary>
        [HttpGet("database-test")]
        public async Task<IActionResult> DatabaseTest()
        {
            try
            {
                // Test basic database connectivity
                var allUsers = await _userDao.GetAllUsersAsync();

                // Test Supabase client connectivity
                var supabaseConnected = _supabaseService.Client != null;

                return Ok(ApiResponse<object>.SuccessResult(new
                {
                    databaseConnectivity = "✅ SUCCESS",
                    supabaseConnectivity = supabaseConnected ? "✅ SUCCESS" : "❌ FAILED",
                    totalUsersInDatabase = allUsers.Count,
                    sampleUsers = allUsers.Take(3).Select(u => new
                    {
                        u.Id,
                        u.Username,
                        u.Email,
                        u.CreatedAt
                    }).ToList(),
                    message = "Database connectivity test completed"
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Database test failed");
                return StatusCode(500, ApiResponse.ErrorResult($"Database test failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// Environment variables check
        /// </summary>
        [HttpGet("env-check")]
        public IActionResult EnvironmentCheck()
        {
            var envVars = new Dictionary<string, object>
            {
                ["SUPABASE_URL"] = new
                {
                    exists = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SUPABASE_URL")),
                    value = Environment.GetEnvironmentVariable("SUPABASE_URL")?.Length > 10
                        ? Environment.GetEnvironmentVariable("SUPABASE_URL")?[..20] + "..."
                        : "NOT SET"
                },
                ["SUPABASE_KEY"] = new
                {
                    exists = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SUPABASE_KEY")),
                    value = Environment.GetEnvironmentVariable("SUPABASE_KEY")?.Length > 10
                        ? Environment.GetEnvironmentVariable("SUPABASE_KEY")?[..20] + "..."
                        : "NOT SET"
                },
                ["SUPABASE_JWT_SECRET"] = new
                {
                    exists = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET")),
                    value = Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET")?.Length > 10
                        ? Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET")?[..20] + "..."
                        : "NOT SET"
                },
                ["ASPNETCORE_ENVIRONMENT"] = new
                {
                    exists = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")),
                    value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "NOT SET"
                }
            };

            var allEnvVarsPresent = envVars.All(kvp =>
                kvp.Key == "ASPNETCORE_ENVIRONMENT" ||
                ((dynamic)kvp.Value).exists == true);

            return Ok(ApiResponse<object>.SuccessResult(new
            {
                environmentCheck = allEnvVarsPresent ? "✅ SUCCESS" : "❌ MISSING VARIABLES",
                variables = envVars,
                message = allEnvVarsPresent
                    ? "All required environment variables are present"
                    : "Some required environment variables are missing"
            }));
        }

        /// <summary>
        /// Complete system health check
        /// </summary>
        [HttpGet("health")]
        public async Task<IActionResult> HealthCheck()
        {
            var checks = new Dictionary<string, object>();
            var overallHealth = true;

            // 1. Environment Variables Check
            try
            {
                var requiredEnvVars = new[] { "SUPABASE_URL", "SUPABASE_KEY", "SUPABASE_JWT_SECRET" };
                var allPresent = requiredEnvVars.All(env => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(env)));
                checks["environmentVariables"] = new { status = allPresent ? "✅ PASS" : "❌ FAIL", healthy = allPresent };
                if (!allPresent) overallHealth = false;
            }
            catch
            {
                checks["environmentVariables"] = new { status = "❌ ERROR", healthy = false };
                overallHealth = false;
            }

            // 2. Database Connectivity Check
            try
            {
                var users = await _userDao.GetAllUsersAsync();
                checks["database"] = new { status = "✅ PASS", healthy = true, userCount = users.Count };
            }
            catch (Exception ex)
            {
                checks["database"] = new { status = "❌ FAIL", healthy = false, error = ex.Message };
                overallHealth = false;
            }

            // 3. Supabase Client Check
            try
            {
                var clientHealthy = _supabaseService.Client != null;
                checks["supabaseClient"] = new { status = clientHealthy ? "✅ PASS" : "❌ FAIL", healthy = clientHealthy };
                if (!clientHealthy) overallHealth = false;
            }
            catch (Exception ex)
            {
                checks["supabaseClient"] = new { status = "❌ ERROR", healthy = false, error = ex.Message };
                overallHealth = false;
            }

            return Ok(ApiResponse<object>.SuccessResult(new
            {
                overallHealth = overallHealth ? "✅ HEALTHY" : "❌ UNHEALTHY",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                checks = checks
            }));
        }

        /// <summary>
        /// Admin-only endpoint to check user synchronization issues
        /// </summary>
        [HttpGet("user-sync-check")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UserSyncCheck()
        {
            try
            {
                var allUsers = await _userDao.GetAllUsersAsync();

                return Ok(ApiResponse<object>.SuccessResult(new
                {
                    totalUsers = allUsers.Count,
                    recentUsers = allUsers
                        .OrderByDescending(u => u.CreatedAt)
                        .Take(10)
                        .Select(u => new
                        {
                            u.Id,
                            u.Username,
                            u.Email,
                            u.Role,
                            u.CreatedAt,
                            DaysOld = (DateTime.UtcNow - u.CreatedAt).Days
                        })
                        .ToList(),
                    message = "User synchronization check completed"
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ User sync check failed");
                return StatusCode(500, ApiResponse.ErrorResult($"User sync check failed: {ex.Message}"));
            }
        }
    }

    public class TokenTestRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}
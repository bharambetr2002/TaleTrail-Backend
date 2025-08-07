// 🔧 UUID Synchronization Controller - One-time fix for existing users
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaleTrail.API.Services;
using TaleTrail.API.DAO;
using TaleTrail.API.Helpers;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/admin/sync")]
    [Authorize(Roles = "admin")] // ADMIN ONLY - Very dangerous operations
    public class UuidSyncController : ControllerBase
    {
        private readonly UserDao _userDao;
        private readonly SupabaseService _supabaseService;
        private readonly ILogger<UuidSyncController> _logger;

        public UuidSyncController(
            UserDao userDao,
            SupabaseService supabaseService,
            ILogger<UuidSyncController> logger)
        {
            _userDao = userDao;
            _supabaseService = supabaseService;
            _logger = logger;
        }

        /// <summary>
        /// ⚠️ DANGER: Analyzes UUID mismatches between Supabase Auth and your users table
        /// </summary>
        [HttpGet("analyze-uuid-mismatches")]
        public async Task<IActionResult> AnalyzeUuidMismatches()
        {
            try
            {
                _logger.LogWarning("🔍 Starting UUID mismatch analysis");

                // Get all users from your database
                var dbUsers = await _userDao.GetAllUsersAsync();

                var report = new
                {
                    totalUsersInDatabase = dbUsers.Count,
                    analysisTimestamp = DateTime.UtcNow,
                    usersAnalyzed = dbUsers.Select(u => new
                    {
                        DatabaseId = u.Id,
                        Email = u.Email,
                        Username = u.Username,
                        CreatedAt = u.CreatedAt,
                        // Note: We can't easily query Supabase Auth users without admin SDK
                        // This would require manual verification
                    }).ToList(),
                    recommendations = new[]
                    {
                        "🔧 For NEW users: Use the fixed AuthService to ensure UUID sync",
                        "🗄️ For EXISTING users: Consider creating new accounts or manual migration",
                        "⚠️ CRITICAL: Do not delete users without backup!"
                    }
                };

                return Ok(ApiResponse<object>.SuccessResult(report, "UUID mismatch analysis completed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to analyze UUID mismatches");
                return StatusCode(500, ApiResponse.ErrorResult($"Analysis failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// ⚠️ EXTREME DANGER: Deletes a user from database ONLY (not Supabase Auth)
        /// Use this ONLY to clean up orphaned database records
        /// </summary>
        [HttpDelete("delete-db-user/{userId}")]
        public async Task<IActionResult> DeleteDatabaseUser(Guid userId, [FromQuery] bool confirmDeletion = false)
        {
            if (!confirmDeletion)
            {
                return BadRequest(ApiResponse.ErrorResult(
                    "⚠️ DANGER: Add ?confirmDeletion=true to confirm. This PERMANENTLY deletes the user from database only!"));
            }

            try
            {
                var user = await _userDao.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(ApiResponse.ErrorResult("User not found in database"));
                }

                _logger.LogWarning("🗑️ DELETING USER FROM DATABASE: {UserId} ({Email}) - Admin action", userId, user.Email);

                await _userDao.DeleteAsync(userId);

                return Ok(ApiResponse.SuccessResult($"User {userId} deleted from database. ⚠️ Supabase Auth record still exists!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to delete user {UserId} from database", userId);
                return StatusCode(500, ApiResponse.ErrorResult($"Deletion failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// 🔧 Creates a user in database with specific UUID (for manual sync)
        /// </summary>
        [HttpPost("create-synced-user")]
        public async Task<IActionResult> CreateSyncedUser([FromBody] CreateSyncedUserRequest request)
        {
            try
            {
                _logger.LogInformation("🔧 Creating synced user with UUID: {UserId}", request.UserId);

                // Check if user already exists
                var existingUser = await _userDao.GetByIdAsync(request.UserId);
                if (existingUser != null)
                {
                    return BadRequest(ApiResponse.ErrorResult($"User with ID {request.UserId} already exists"));
                }

                // Check if email/username already exists
                if (await _userDao.EmailExistsAsync(request.Email))
                {
                    return BadRequest(ApiResponse.ErrorResult($"Email {request.Email} already exists"));
                }

                if (await _userDao.UsernameExistsAsync(request.Username))
                {
                    return BadRequest(ApiResponse.ErrorResult($"Username {request.Username} already exists"));
                }

                var newUser = new TaleTrail.API.Models.User
                {
                    Id = request.UserId, // Use provided UUID
                    Email = request.Email.ToLowerInvariant(),
                    FullName = request.FullName,
                    Username = request.Username.ToLowerInvariant(),
                    CreatedAt = DateTime.UtcNow,
                    Role = "user"
                };

                var createdUser = await _userDao.AddAsync(newUser);
                if (createdUser == null)
                {
                    return StatusCode(500, ApiResponse.ErrorResult("Failed to create user"));
                }

                _logger.LogInformation("✅ Successfully created synced user: {UserId} ({Username})", createdUser.Id, createdUser.Username);

                return Ok(ApiResponse<object>.SuccessResult(createdUser, "Synced user created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to create synced user");
                return StatusCode(500, ApiResponse.ErrorResult($"User creation failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// 📊 Gets comprehensive system sync status
        /// </summary>
        [HttpGet("system-status")]
        public async Task<IActionResult> GetSystemSyncStatus()
        {
            try
            {
                var dbUsers = await _userDao.GetAllUsersAsync();

                var status = new
                {
                    timestamp = DateTime.UtcNow,
                    database = new
                    {
                        totalUsers = dbUsers.Count,
                        recentUsers = dbUsers.OrderByDescending(u => u.CreatedAt).Take(5).Select(u => new
                        {
                            u.Id,
                            u.Username,
                            u.Email,
                            u.CreatedAt
                        })
                    },
                    recommendations = new[]
                    {
                        "✅ All NEW signups will use synchronized UUIDs",
                        "🔧 Existing users may need manual verification",
                        "📋 Test login/profile access for existing users",
                        "⚠️ Consider asking users to re-register if they have issues"
                    }
                };

                return Ok(ApiResponse<object>.SuccessResult(status, "System sync status retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to get system status");
                return StatusCode(500, ApiResponse.ErrorResult($"Status check failed: {ex.Message}"));
            }
        }
    }

    public class CreateSyncedUserRequest
    {
        public Guid UserId { get; set; } // The Supabase Auth UUID
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
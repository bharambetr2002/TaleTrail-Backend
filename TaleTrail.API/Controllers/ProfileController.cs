using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(UserService userService, ILogger<ProfileController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all public data for a user's profile page, including stats and book lists.
        /// This endpoint is public and does not require authentication.
        /// </summary>
        /// <param name="username">The unique username of the user.</param>
        /// <returns>A PublicProfileDto with all the user's public information.</returns>
        [HttpGet("{username}")]
        public async Task<IActionResult> GetPublicProfile(string username)
        {
            try
            {
                var profile = await _userService.GetPublicProfileByUsernameAsync(username);
                if (profile == null)
                {
                    return NotFound(ApiResponse.ErrorResult("User profile not found."));
                }
                return Ok(ApiResponse<object>.SuccessResult(profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting public profile for username {Username}", username);
                return BadRequest(ApiResponse.ErrorResult($"An error occurred while fetching the profile: {ex.Message}"));
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly UserService _userService;

        public ProfileController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetPublicProfile(string username)
        {
            var profile = await _userService.GetPublicProfileByUsernameAsync(username);
            if (profile == null)
            {
                return NotFound(ApiResponse.ErrorResult("User profile not found."));
            }
            return Ok(ApiResponse<object>.SuccessResult(profile));
        }
    }
}
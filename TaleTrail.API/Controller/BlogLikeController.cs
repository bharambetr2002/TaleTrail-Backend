using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/blog-like")]
[Authorize]
public class BlogLikeController : BaseController
{
    private readonly BlogService _blogService;

    public BlogLikeController(UserService userService, BlogService blogService, ILogger<BlogLikeController> logger)
        : base(userService, logger)
    {
        _blogService = blogService;
    }

    [HttpPost("{blogId}")]
    public async Task<IActionResult> Like(Guid blogId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _blogService.LikeBlogAsync(blogId, userId);
            return Ok(ApiResponse<string?>.SuccessResponse("Blog liked successfully", null));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string?>.ErrorResponse($"Failed to like blog: {ex.Message}"));
        }
    }

    [HttpDelete("{blogId}")]
    public async Task<IActionResult> Unlike(Guid blogId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _blogService.UnlikeBlogAsync(blogId, userId);
            return Ok(ApiResponse<string?>.SuccessResponse("Blog unliked successfully", null));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string?>.ErrorResponse($"Failed to unlike blog: {ex.Message}"));
        }
    }

    [HttpGet("{blogId}/count")]
    public async Task<IActionResult> GetLikeCount(Guid blogId)
    {
        try
        {
            var count = await _blogService.GetLikeCountAsync(blogId);
            return Ok(ApiResponse<int>.SuccessResponse("Like count retrieved successfully", count));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<int>.ErrorResponse($"Failed to get like count: {ex.Message}"));
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model;
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
        var userId = GetCurrentUserId();
        var like = await _blogService.LikeBlogAsync(blogId, userId);
        return Ok(ApiResponse<BlogLike>.SuccessResponse("Blog liked", like));
    }

    [HttpDelete("{blogId}")]
    public async Task<IActionResult> Unlike(Guid blogId)
    {
        var userId = GetCurrentUserId();
        await _blogService.UnlikeBlogAsync(blogId, userId);
        return Ok(ApiResponse<string?>.SuccessResponse("Blog unliked", null));
    }

    [HttpGet("{blogId}/count")]
    public async Task<IActionResult> GetLikeCount(Guid blogId)
    {
        var count = await _blogService.GetLikeCountAsync(blogId);
        return Ok(ApiResponse<int>.SuccessResponse("Fetched like count", count));
    }
}
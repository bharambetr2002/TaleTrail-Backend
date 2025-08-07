using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;
using System;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/blog-like")]
    [Authorize]
    public class BlogLikeController : BaseController
    {
        private readonly BlogLikeService _blogLikeService;

        public BlogLikeController(BlogLikeService blogLikeService)
        {
            _blogLikeService = blogLikeService;
        }

        [HttpPost("{blogId}")]
        public async Task<IActionResult> LikeBlog(Guid blogId)
        {
            var userId = GetCurrentUserId();
            var blogLike = await _blogLikeService.LikeBlogAsync(blogId, userId);
            return Ok(ApiResponse<object>.SuccessResult(blogLike));
        }

        [HttpDelete("{blogId}")]
        public async Task<IActionResult> UnlikeBlog(Guid blogId)
        {
            var userId = GetCurrentUserId();
            await _blogLikeService.UnlikeBlogAsync(blogId, userId);
            return Ok(ApiResponse.SuccessResult("Blog unliked."));
        }

        [HttpGet("{blogId}/status")]
        public async Task<IActionResult> GetLikeStatus(Guid blogId)
        {
            var userId = GetCurrentUserId();
            var hasLiked = await _blogLikeService.HasUserLikedBlogAsync(blogId, userId);
            return Ok(ApiResponse<object>.SuccessResult(new { hasLiked }));
        }
    }
}
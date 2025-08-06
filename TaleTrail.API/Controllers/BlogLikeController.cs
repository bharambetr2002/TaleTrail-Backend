using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/blog-like")]
    [Authorize] // All blog like operations require authentication
    public class BlogLikeController : BaseController
    {
        private readonly BlogLikeService _blogLikeService;
        private readonly ILogger<BlogLikeController> _logger;

        public BlogLikeController(BlogLikeService blogLikeService, ILogger<BlogLikeController> logger)
        {
            _blogLikeService = blogLikeService;
            _logger = logger;
        }

        /// <summary>
        /// Like a blog post
        /// </summary>
        [HttpPost("{blogId}")]
        public async Task<IActionResult> LikeBlog(Guid blogId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var blogLike = await _blogLikeService.LikeBlogAsync(blogId, userId);
                return Ok(ApiResponse<object>.SuccessResult(blogLike, "Blog liked successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking blog {BlogId}", blogId);
                return BadRequest(ApiResponse.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Unlike a blog post
        /// </summary>
        [HttpDelete("{blogId}")]
        public async Task<IActionResult> UnlikeBlog(Guid blogId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _blogLikeService.UnlikeBlogAsync(blogId, userId);

                if (!success)
                {
                    return NotFound(ApiResponse.ErrorResult("You have not liked this blog or the blog does not exist"));
                }

                return Ok(ApiResponse.SuccessResult("Blog unliked successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unliking blog {BlogId}", blogId);
                return BadRequest(ApiResponse.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Check if current user has liked a blog
        /// </summary>
        [HttpGet("{blogId}/status")]
        public async Task<IActionResult> GetLikeStatus(Guid blogId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var hasLiked = await _blogLikeService.HasUserLikedBlogAsync(blogId, userId);
                return Ok(ApiResponse<object>.SuccessResult(new { hasLiked }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking like status for blog {BlogId}", blogId);
                return BadRequest(ApiResponse.ErrorResult(ex.Message));
            }
        }
    }
}
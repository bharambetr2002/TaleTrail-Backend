using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : BaseController
    {
        private readonly BlogService _blogService;
        private readonly ILogger<BlogController> _logger;

        public BlogController(BlogService blogService, ILogger<BlogController> logger)
        {
            _blogService = blogService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogs([FromQuery] Guid? userId = null)
        {
            try
            {
                var blogs = await _blogService.GetBlogsAsync(userId);
                return Ok(ApiResponse<object>.SuccessResult(blogs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting blogs");
                return BadRequest(ApiResponse.ErrorResult($"Error getting blogs: {ex.Message}"));
            }
        }

        [HttpGet("my-blogs")]
        [Authorize]
        public async Task<IActionResult> GetMyBlogs()
        {
            try
            {
                var userId = GetCurrentUserId();
                var blogs = await _blogService.GetBlogsAsync(userId);
                return Ok(ApiResponse<object>.SuccessResult(blogs, $"Found {blogs.Count} blogs"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user's blogs");
                return BadRequest(ApiResponse.ErrorResult($"Error getting blogs: {ex.Message}"));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlog([FromBody] BlogDto blogDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var blog = await _blogService.CreateBlogAsync(blogDto, userId);
                return CreatedAtAction(nameof(GetBlogs), new { id = blog.Id }, ApiResponse<object>.SuccessResult(blog, "Blog created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating blog");
                return BadRequest(ApiResponse.ErrorResult($"Failed to create blog: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlog(Guid id, [FromBody] BlogDto blogDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var blog = await _blogService.UpdateBlogAsync(id, blogDto, userId);

                if (blog == null)
                {
                    // CORRECTED LINE: Pass the error message string directly to Forbid()
                    return Forbid("You are not authorized to update this blog or it does not exist.");
                }

                return Ok(ApiResponse<object>.SuccessResult(blog, "Blog updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blog {BlogId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Failed to update blog: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlog(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _blogService.DeleteBlogAsync(id, userId);

                if (!success)
                {
                    // CORRECTED LINE: Pass the error message string directly to Forbid()
                    return Forbid("You are not authorized to delete this blog or it does not exist.");
                }

                return Ok(ApiResponse.SuccessResult("Blog deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blog {BlogId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Failed to delete blog: {ex.Message}"));
            }
        }
    }
}
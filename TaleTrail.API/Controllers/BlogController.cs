using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;

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
        public async Task<IActionResult> GetAllBlogs()
        {
            try
            {
                var blogs = await _blogService.GetAllBlogsAsync();
                return Ok(ApiResponse<object>.SuccessResult(blogs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all blogs");
                return BadRequest(ApiResponse.ErrorResult($"Error getting blogs: {ex.Message}"));
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserBlogs(Guid userId)
        {
            try
            {
                var blogs = await _blogService.GetUserBlogsAsync(userId);
                return Ok(ApiResponse<object>.SuccessResult(blogs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user blogs for user {UserId}", userId);
                return BadRequest(ApiResponse.ErrorResult($"Error getting user blogs: {ex.Message}"));
            }
        }

        [HttpGet("user/my-blogs")]
        public async Task<IActionResult> GetMyBlogs()
        {
            try
            {
                var userId = GetCurrentUserId();
                var blogs = await _blogService.GetUserBlogsAsync(userId);
                return Ok(ApiResponse<object>.SuccessResult(blogs, $"Found {blogs.Count} blogs"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to get user blogs");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user blogs");
                return BadRequest(ApiResponse.ErrorResult($"Error getting blogs: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlog([FromBody] BlogDto blogDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                // Get user ID from JWT token via middleware
                var userId = GetCurrentUserId();

                var blog = await _blogService.CreateBlogAsync(blogDto, userId);
                return Ok(ApiResponse<object>.SuccessResult(blog, "Blog created successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized blog creation attempt");
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating blog");
                return BadRequest(ApiResponse.ErrorResult($"Failed to create blog: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(Guid id, [FromBody] BlogDto blogDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                // Get user ID from JWT token via middleware
                var userId = GetCurrentUserId();

                var blog = await _blogService.UpdateBlogAsync(id, blogDto, userId);
                return Ok(ApiResponse<object>.SuccessResult(blog, "Blog updated successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized blog update attempt for blog {BlogId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blog {BlogId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Failed to update blog: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(Guid id)
        {
            try
            {
                // Get user ID from JWT token via middleware
                var userId = GetCurrentUserId();

                await _blogService.DeleteBlogAsync(id, userId);
                return Ok(ApiResponse.SuccessResult("Blog deleted successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized blog deletion attempt for blog {BlogId}", id);
                return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blog {BlogId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Failed to delete blog: {ex.Message}"));
            }
        }
    }
}
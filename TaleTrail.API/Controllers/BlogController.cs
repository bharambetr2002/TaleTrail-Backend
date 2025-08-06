using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly BlogService _blogService;
        private readonly SupabaseService _supabase;
        private readonly ILogger<BlogController> _logger;

        public BlogController(BlogService blogService, SupabaseService supabase, ILogger<BlogController> logger)
        {
            _blogService = blogService;
            _supabase = supabase;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _blogService.GetAllBlogsAsync();
            return Ok(ApiResponse<object>.SuccessResult(blogs));
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserBlogs(Guid userId)
        {
            var blogs = await _blogService.GetUserBlogsAsync(userId);
            return Ok(ApiResponse<object>.SuccessResult(blogs));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlog([FromBody] CreateBlogRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                // Validate that the user exists before creating the blog
                var userResponse = await _supabase.Client
                    .From<TaleTrail.API.Models.User>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.UserId.ToString())
                    .Get();

                if (!userResponse.Models?.Any() == true)
                {
                    return BadRequest(ApiResponse.ErrorResult($"User with ID {request.UserId} does not exist"));
                }

                var blog = await _blogService.CreateBlogAsync(request.BlogDto, request.UserId);
                return Ok(ApiResponse<object>.SuccessResult(blog, "Blog created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating blog for user {UserId}", request.UserId);
                return BadRequest(ApiResponse.ErrorResult($"Failed to create blog: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(Guid id, [FromBody] CreateBlogRequest request)
        {
            try
            {
                var blog = await _blogService.UpdateBlogAsync(id, request.BlogDto, request.UserId);
                return Ok(ApiResponse<object>.SuccessResult(blog, "Blog updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blog {BlogId}", id);
                return BadRequest(ApiResponse.ErrorResult($"Failed to update blog: {ex.Message}"));
            }
        }
    }

    // Add this new request model to handle both userId and blogDto
    public class CreateBlogRequest
    {
        public Guid UserId { get; set; }
        public BlogDto BlogDto { get; set; } = new BlogDto();
    }
}
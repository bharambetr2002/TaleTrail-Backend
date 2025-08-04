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

        public BlogController(BlogService blogService)
        {
            _blogService = blogService;
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
        public async Task<IActionResult> CreateBlog([FromBody] BlogDto blogDto)
        {
            // In a real app, you'd get userId from JWT token
            var userId = Guid.NewGuid(); // Placeholder
            var blog = await _blogService.CreateBlogAsync(blogDto, userId);
            return Ok(ApiResponse<object>.SuccessResult(blog, "Blog created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(Guid id, [FromBody] BlogDto blogDto)
        {
            // In a real app, you'd get userId from JWT token
            var userId = Guid.NewGuid(); // Placeholder
            var blog = await _blogService.UpdateBlogAsync(id, blogDto, userId);
            return Ok(ApiResponse<object>.SuccessResult(blog, "Blog updated successfully"));
        }
    }
}
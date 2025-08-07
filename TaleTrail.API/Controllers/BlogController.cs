using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;
using System;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : BaseController
    {
        private readonly BlogService _blogService;

        public BlogController(BlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogs([FromQuery] Guid? userId = null)
        {
            var blogs = await _blogService.GetBlogsAsync(userId);
            return Ok(ApiResponse<object>.SuccessResult(blogs));
        }

        [HttpGet("my-blogs")]
        [Authorize]
        public async Task<IActionResult> GetMyBlogs()
        {
            var userId = GetCurrentUserId();
            var blogs = await _blogService.GetBlogsAsync(userId);
            return Ok(ApiResponse<object>.SuccessResult(blogs));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlog([FromBody] BlogDto blogDto)
        {
            var userId = GetCurrentUserId();
            var blog = await _blogService.CreateBlogAsync(blogDto, userId);
            return CreatedAtAction(nameof(GetBlogs), new { id = blog.Id }, ApiResponse<object>.SuccessResult(blog));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlog(Guid id, [FromBody] BlogDto blogDto)
        {
            var userId = GetCurrentUserId();
            var blog = await _blogService.UpdateBlogAsync(id, blogDto, userId);

            if (blog == null)
            {
                return Forbid();
            }

            return Ok(ApiResponse<object>.SuccessResult(blog));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlog(Guid id)
        {
            var userId = GetCurrentUserId();
            var success = await _blogService.DeleteBlogAsync(id, userId);

            if (!success)
            {
                return Forbid();
            }

            return Ok(ApiResponse.SuccessResult("Blog deleted."));
        }
    }
}
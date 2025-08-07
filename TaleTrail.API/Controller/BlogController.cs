using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController : BaseController
{
    private readonly BlogService _blogService;

    public BlogController(UserService userService, BlogService blogService, ILogger<BlogController> logger)
        : base(userService, logger)
    {
        _blogService = blogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? userId = null)
    {
        List<Blog> blogs;
        if (userId.HasValue)
            blogs = await _blogService.GetBlogsByUserIdAsync(userId.Value);
        else
            blogs = await _blogService.GetAllBlogsAsync();

        return Ok(ApiResponse<List<Blog>>.SuccessResponse("Fetched blogs", blogs));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var blog = await _blogService.GetBlogByIdAsync(id);
        if (blog == null)
            return NotFound(ApiResponse<Blog>.ErrorResponse("Blog not found"));

        return Ok(ApiResponse<Blog>.SuccessResponse("Fetched blog", blog));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateBlogRequest request)
    {
        var userId = GetCurrentUserId();
        var blog = await _blogService.CreateBlogAsync(userId, request);
        return Ok(ApiResponse<Blog>.SuccessResponse("Blog created", blog));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBlogRequest request)
    {
        var userId = GetCurrentUserId();
        var blog = await _blogService.UpdateBlogAsync(id, userId, request);
        return Ok(ApiResponse<Blog>.SuccessResponse("Blog updated", blog));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();
        await _blogService.DeleteBlogAsync(id, userId);
        return Ok(ApiResponse<string?>.SuccessResponse("Blog deleted", null));
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
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
        try
        {
            var blogs = userId.HasValue
                ? await _blogService.GetBlogsByUserIdAsync(userId.Value)
                : await _blogService.GetAllBlogsAsync();

            var message = userId.HasValue
                ? $"Retrieved {blogs.Count} blogs for user"
                : $"Retrieved {blogs.Count} blogs";

            return Ok(ApiResponse<List<BlogResponseDTO>>.SuccessResponse(message, blogs));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<BlogResponseDTO>>.ErrorResponse($"Failed to retrieve blogs: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var blog = await _blogService.GetBlogByIdAsync(id);
            if (blog == null)
                return NotFound(ApiResponse<BlogResponseDTO>.ErrorResponse("Blog not found"));

            return Ok(ApiResponse<BlogResponseDTO>.SuccessResponse("Blog retrieved successfully", blog));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<BlogResponseDTO>.ErrorResponse($"Failed to retrieve blog: {ex.Message}"));
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateBlogRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var blog = await _blogService.CreateBlogAsync(userId, request);
            return Ok(ApiResponse<BlogResponseDTO>.SuccessResponse("Blog created successfully", blog));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<BlogResponseDTO>.ErrorResponse($"Failed to create blog: {ex.Message}"));
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBlogRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var blog = await _blogService.UpdateBlogAsync(id, userId, request);
            return Ok(ApiResponse<BlogResponseDTO>.SuccessResponse("Blog updated successfully", blog));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<BlogResponseDTO>.ErrorResponse($"Failed to update blog: {ex.Message}"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _blogService.DeleteBlogAsync(id, userId);
            return Ok(ApiResponse<string?>.SuccessResponse("Blog deleted successfully", null));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string?>.ErrorResponse($"Failed to delete blog: {ex.Message}"));
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : BaseController
{
    private readonly ReviewService _reviewService;

    public ReviewController(UserService userService, ReviewService reviewService)
        : base(userService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("book/{bookId}")]
    public async Task<IActionResult> GetByBookId(Guid bookId)
    {
        var reviews = await _reviewService.GetReviewsByBookIdAsync(bookId);
        return Ok(ApiResponse<List<Review>>.SuccessResponse("Fetched reviews", reviews));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
    {
        var userId = GetCurrentUserId();
        var review = await _reviewService.CreateReviewAsync(userId, request);
        return Ok(ApiResponse<Review>.SuccessResponse("Review created", review));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewRequest request)
    {
        var userId = GetCurrentUserId();
        var review = await _reviewService.UpdateReviewAsync(id, userId, request);
        return Ok(ApiResponse<Review>.SuccessResponse("Review updated", review));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();
        await _reviewService.DeleteReviewAsync(id, userId);
        return Ok(ApiResponse<object>.SuccessResponse("Review deleted", null));
    }
}
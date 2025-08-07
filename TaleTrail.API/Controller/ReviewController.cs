using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : BaseController
{
    private readonly ReviewService _reviewService;

    public ReviewController(UserService userService, ReviewService reviewService, ILogger<ReviewController> logger)
        : base(userService, logger)
    {
        _reviewService = reviewService;
    }

    [HttpGet("book/{bookId}")]
    public async Task<IActionResult> GetByBookId(Guid bookId)
    {
        try
        {
            var reviews = await _reviewService.GetReviewsByBookIdAsync(bookId);
            return Ok(ApiResponse<List<ReviewResponseDTO>>.SuccessResponse($"Retrieved {reviews.Count} reviews for book", reviews));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<ReviewResponseDTO>>.ErrorResponse($"Failed to retrieve reviews: {ex.Message}"));
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequestDTO request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var review = await _reviewService.CreateReviewAsync(userId, request);
            return Ok(ApiResponse<ReviewResponseDTO>.SuccessResponse("Review created successfully", review));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ReviewResponseDTO>.ErrorResponse($"Failed to create review: {ex.Message}"));
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewRequestDTO request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var review = await _reviewService.UpdateReviewAsync(id, userId, request);
            return Ok(ApiResponse<ReviewResponseDTO>.SuccessResponse("Review updated successfully", review));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ReviewResponseDTO>.ErrorResponse($"Failed to update review: {ex.Message}"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _reviewService.DeleteReviewAsync(id, userId);
            return Ok(ApiResponse<string?>.SuccessResponse("Review deleted successfully", null));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string?>.ErrorResponse($"Failed to delete review: {ex.Message}"));
        }
    }
}
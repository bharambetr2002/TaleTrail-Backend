using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublisherController : ControllerBase
{
    private readonly PublisherService _publisherService;

    public PublisherController(PublisherService publisherService)
    {
        _publisherService = publisherService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var publishers = await _publisherService.GetAllPublishersAsync();
            return Ok(ApiResponse<List<PublisherResponseDTO>>.SuccessResponse($"Retrieved {publishers.Count} publishers", publishers));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<PublisherResponseDTO>>.ErrorResponse($"Failed to retrieve publishers: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(id);
            if (publisher == null)
                return NotFound(ApiResponse<PublisherResponseDTO>.ErrorResponse("Publisher not found"));

            return Ok(ApiResponse<PublisherResponseDTO>.SuccessResponse("Publisher retrieved successfully", publisher));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PublisherResponseDTO>.ErrorResponse($"Failed to retrieve publisher: {ex.Message}"));
        }
    }
}
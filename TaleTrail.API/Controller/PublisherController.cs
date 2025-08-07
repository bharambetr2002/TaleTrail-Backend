using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model;
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
        var publishers = await _publisherService.GetAllPublishersAsync();
        return Ok(ApiResponse<List<Publisher>>.SuccessResponse("Fetched all publishers", publishers));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var publisher = await _publisherService.GetPublisherByIdAsync(id);
        if (publisher == null)
            return NotFound(ApiResponse<Publisher>.ErrorResponse("Publisher not found"));

        return Ok(ApiResponse<Publisher>.SuccessResponse("Fetched publisher", publisher));
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Add this for authorization
using TaleTrail.API.Services;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;
using System;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublisherController : BaseController
    {
        private readonly PublisherService _publisherService;
        private readonly ILogger<PublisherController> _logger;

        public PublisherController(PublisherService publisherService, ILogger<PublisherController> logger)
        {
            _publisherService = publisherService;
            _logger = logger;
        }

        // GET: api/publisher - This is public
        [HttpGet]
        public async Task<IActionResult> GetAllPublishers()
        {
            try
            {
                var publishers = await _publisherService.GetAllPublishersAsync();
                return Ok(ApiResponse<object>.SuccessResult(publishers));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all publishers");
                return BadRequest(ApiResponse.ErrorResult($"Error getting publishers: {ex.Message}"));
            }
        }

        // GET: api/publisher/{id} - This is public
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPublisherById(Guid id)
        {
            try
            {
                var publisher = await _publisherService.GetPublisherByIdAsync(id);
                if (publisher == null)
                    return NotFound(ApiResponse.ErrorResult("Publisher not found"));

                return Ok(ApiResponse<object>.SuccessResult(publisher));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting publisher {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error getting publisher: {ex.Message}"));
            }
        }

        // POST: api/publisher - ADMIN ONLY
        [HttpPost]
        [Authorize(Roles = "admin")] // IMPORTANT: Secures the endpoint
        public async Task<IActionResult> CreatePublisher([FromBody] PublisherDto publisherDto)
        {
            try
            {
                var newPublisher = await _publisherService.CreatePublisherAsync(publisherDto);
                return CreatedAtAction(nameof(GetPublisherById), new { id = newPublisher.Id }, ApiResponse<object>.SuccessResult(newPublisher, "Publisher created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating publisher");
                return BadRequest(ApiResponse.ErrorResult($"Error creating publisher: {ex.Message}"));
            }
        }

        // PUT: api/publisher/{id} - ADMIN ONLY
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")] // IMPORTANT: Secures the endpoint
        public async Task<IActionResult> UpdatePublisher(Guid id, [FromBody] PublisherDto publisherDto)
        {
            try
            {
                var updatedPublisher = await _publisherService.UpdatePublisherAsync(id, publisherDto);
                if (updatedPublisher == null)
                    return NotFound(ApiResponse.ErrorResult("Publisher not found"));

                return Ok(ApiResponse<object>.SuccessResult(updatedPublisher, "Publisher updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating publisher {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error updating publisher: {ex.Message}"));
            }
        }

        // DELETE: api/publisher/{id} - ADMIN ONLY
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")] // IMPORTANT: Secures the endpoint
        public async Task<IActionResult> DeletePublisher(Guid id)
        {
            try
            {
                var success = await _publisherService.DeletePublisherAsync(id);
                if (!success)
                    return NotFound(ApiResponse.ErrorResult("Publisher not found"));

                return Ok(ApiResponse.SuccessResult("Publisher deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting publisher {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error deleting publisher: {ex.Message}"));
            }
        }
    }
}
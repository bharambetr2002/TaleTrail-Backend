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
    public class PublisherController : BaseController
    {
        private readonly PublisherService _publisherService;

        public PublisherController(PublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPublishers()
        {
            var publishers = await _publisherService.GetAllPublishersAsync();
            return Ok(ApiResponse<object>.SuccessResult(publishers));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPublisherById(Guid id)
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(id);
            if (publisher == null)
                return NotFound(ApiResponse.ErrorResult("Publisher not found"));

            return Ok(ApiResponse<object>.SuccessResult(publisher));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreatePublisher([FromBody] PublisherDto publisherDto)
        {
            var newPublisher = await _publisherService.CreatePublisherAsync(publisherDto);
            return CreatedAtAction(nameof(GetPublisherById), new { id = newPublisher.Id }, ApiResponse<object>.SuccessResult(newPublisher));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdatePublisher(Guid id, [FromBody] PublisherDto publisherDto)
        {
            var updatedPublisher = await _publisherService.UpdatePublisherAsync(id, publisherDto);
            if (updatedPublisher == null)
                return NotFound(ApiResponse.ErrorResult("Publisher not found"));

            return Ok(ApiResponse<object>.SuccessResult(updatedPublisher));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletePublisher(Guid id)
        {
            var success = await _publisherService.DeletePublisherAsync(id);
            if (!success)
                return NotFound(ApiResponse.ErrorResult("Publisher not found"));

            return Ok(ApiResponse.SuccessResult("Publisher deleted."));
        }
    }
}
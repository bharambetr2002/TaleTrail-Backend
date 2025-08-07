using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
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
    }
}
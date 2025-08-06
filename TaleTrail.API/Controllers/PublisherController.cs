using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublisherController : ControllerBase
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<PublisherController> _logger;

        public PublisherController(SupabaseService supabase, ILogger<PublisherController> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        // GET: api/publisher
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _supabase.Client.From<Publisher>().Get();
                var publishers = response.Models?.Select(p => new
                {
                    p.Id,
                    p.Name
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(publishers, $"Found {publishers?.Count ?? 0} publishers"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all publishers");
                return BadRequest(ApiResponse.ErrorResult($"Error getting publishers: {ex.Message}"));
            }
        }

        // GET: api/publisher/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var response = await _supabase.Client
                    .From<Publisher>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                var publisher = response.Models?.FirstOrDefault();
                if (publisher == null)
                    return NotFound(ApiResponse.ErrorResult("Publisher not found"));

                var result = new { publisher.Id, publisher.Name };
                return Ok(ApiResponse<object>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting publisher {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error getting publisher: {ex.Message}"));
            }
        }

        // POST: api/publisher
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PublisherDto publisherDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var publisher = new Publisher
                {
                    Id = Guid.NewGuid(),
                    Name = publisherDto.Name
                };

                var response = await _supabase.Client.From<Publisher>().Insert(publisher);
                var created = response.Models?.FirstOrDefault();

                if (created == null)
                    return BadRequest(ApiResponse.ErrorResult("Failed to create publisher"));

                var result = new { created.Id, created.Name };
                return Ok(ApiResponse<object>.SuccessResult(result, "Publisher created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating publisher");
                return BadRequest(ApiResponse.ErrorResult($"Error creating publisher: {ex.Message}"));
            }
        }

        // PUT: api/publisher/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PublisherDto publisherDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var updated = new Publisher
                {
                    Id = id,
                    Name = publisherDto.Name
                };

                var response = await _supabase.Client.From<Publisher>().Update(updated);
                var publisher = response.Models?.FirstOrDefault();

                if (publisher == null)
                    return NotFound(ApiResponse.ErrorResult("Publisher not found or update failed"));

                var result = new { publisher.Id, publisher.Name };
                return Ok(ApiResponse<object>.SuccessResult(result, "Publisher updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating publisher {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error updating publisher: {ex.Message}"));
            }
        }

        // DELETE: api/publisher/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var record = new Publisher { Id = id };
                await _supabase.Client.From<Publisher>().Delete(record);

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
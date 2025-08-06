using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(SupabaseService supabase, ILogger<AuthorController> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        // GET: api/author
        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            try
            {
                var response = await _supabase.Client.From<Author>().Get();
                var authors = response.Models?.Select(a => new
                {
                    a.Id,
                    a.Name
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(authors, $"Found {authors?.Count ?? 0} authors"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all authors");
                return BadRequest(ApiResponse.ErrorResult($"Error getting authors: {ex.Message}"));
            }
        }

        // GET: api/author/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            try
            {
                var response = await _supabase.Client
                    .From<Author>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                var author = response.Models?.FirstOrDefault();
                if (author == null)
                    return NotFound(ApiResponse.ErrorResult("Author not found"));

                var authorDto = new { author.Id, author.Name };
                return Ok(ApiResponse<object>.SuccessResult(authorDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting author {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error getting author: {ex.Message}"));
            }
        }

        // POST: api/author
        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorDto authorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var author = new Author
                {
                    Id = Guid.NewGuid(),
                    Name = authorDto.Name
                };

                var response = await _supabase.Client.From<Author>().Insert(author);
                var createdAuthor = response.Models?.FirstOrDefault();

                if (createdAuthor == null)
                    return BadRequest(ApiResponse.ErrorResult("Failed to create author"));

                var result = new { createdAuthor.Id, createdAuthor.Name };
                return Ok(ApiResponse<object>.SuccessResult(result, "Author created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating author");
                return BadRequest(ApiResponse.ErrorResult($"Error creating author: {ex.Message}"));
            }
        }

        // PUT: api/author/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(Guid id, [FromBody] AuthorDto authorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var updatedAuthor = new Author
                {
                    Id = id,
                    Name = authorDto.Name
                };

                var response = await _supabase.Client.From<Author>().Update(updatedAuthor);
                var author = response.Models?.FirstOrDefault();

                if (author == null)
                    return NotFound(ApiResponse.ErrorResult("Author not found or update failed"));

                var result = new { author.Id, author.Name };
                return Ok(ApiResponse<object>.SuccessResult(result, "Author updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating author {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error updating author: {ex.Message}"));
            }
        }

        // DELETE: api/author/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            try
            {
                var author = new Author { Id = id };
                await _supabase.Client.From<Author>().Delete(author);

                return Ok(ApiResponse.SuccessResult("Author deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting author {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error deleting author: {ex.Message}"));
            }
        }
    }
}
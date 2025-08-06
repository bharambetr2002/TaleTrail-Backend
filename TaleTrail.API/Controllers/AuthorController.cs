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
    public class AuthorController : ControllerBase // Inherit from BaseController if it has helper methods
    {
        private readonly AuthorService _authorService;

        public AuthorController(AuthorService authorService)
        {
            _authorService = authorService;
        }

        // GET: api/author - This remains public
        [HttpGet]
        public async Task<IActionResult> GetAllAuthors([FromQuery] string? search = null)
        {
            var authors = await _authorService.GetAllAuthorsAsync(search);
            return Ok(ApiResponse<object>.SuccessResult(authors, $"Found {authors.Count} authors"));
        }

        // GET: api/author/{id} - This remains public
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
                return NotFound(ApiResponse.ErrorResult("Author not found"));

            return Ok(ApiResponse<object>.SuccessResult(author));
        }

        // POST: api/author - This is now protected
        [HttpPost]
        [Authorize(Roles = "admin")] // IMPORTANT: Only allows users with the 'admin' role
        public async Task<IActionResult> AddAuthor([FromBody] AuthorDto authorDto)
        {
            var newAuthor = await _authorService.AddAuthorAsync(authorDto);
            return CreatedAtAction(nameof(GetAuthorById), new { id = newAuthor.Id }, ApiResponse<object>.SuccessResult(newAuthor, "Author created successfully"));
        }

        // PUT: api/author/{id} - This is now protected
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")] // IMPORTANT: Only allows users with the 'admin' role
        public async Task<IActionResult> UpdateAuthor(Guid id, [FromBody] AuthorDto authorDto)
        {
            var updatedAuthor = await _authorService.UpdateAuthorAsync(id, authorDto);
            if (updatedAuthor == null)
                return NotFound(ApiResponse.ErrorResult("Author not found"));

            return Ok(ApiResponse<object>.SuccessResult(updatedAuthor, "Author updated successfully"));
        }

        // DELETE: api/author/{id} - This is now protected
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")] // IMPORTANT: Only allows users with the 'admin' role
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            var success = await _authorService.DeleteAuthorAsync(id);
            if (!success)
                return NotFound(ApiResponse.ErrorResult("Author not found"));

            return Ok(ApiResponse.SuccessResult("Author deleted successfully"));
        }
    }
}
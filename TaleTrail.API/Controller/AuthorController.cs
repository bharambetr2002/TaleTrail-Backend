using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly AuthorService _authorService;

    public AuthorController(AuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(ApiResponse<List<AuthorResponseDTO>>.SuccessResponse($"Retrieved {authors.Count} authors", authors));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<AuthorResponseDTO>>.ErrorResponse($"Failed to retrieve authors: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
                return NotFound(ApiResponse<AuthorResponseDTO>.ErrorResponse("Author not found"));

            return Ok(ApiResponse<AuthorResponseDTO>.SuccessResponse("Author retrieved successfully", author));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AuthorResponseDTO>.ErrorResponse($"Failed to retrieve author: {ex.Message}"));
        }
    }
}
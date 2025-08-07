using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Helpers;
using TaleTrail.API.Model;
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
        var authors = await _authorService.GetAllAuthorsAsync();
        return Ok(ApiResponse<List<Author>>.SuccessResponse("Fetched all authors", authors));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
            return NotFound(ApiResponse<Author>.ErrorResponse("Author not found"));

        return Ok(ApiResponse<Author>.SuccessResponse("Fetched author", author));
    }
}

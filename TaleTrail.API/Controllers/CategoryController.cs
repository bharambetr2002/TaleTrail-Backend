using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Services;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(SupabaseService supabase, ILogger<CategoryController> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        // GET: api/category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _supabase.Client.From<Category>().Get();
                var categories = response.Models?.Select(c => new
                {
                    c.Id,
                    c.Name
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResult(categories, $"Found {categories?.Count ?? 0} categories"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all categories");
                return BadRequest(ApiResponse.ErrorResult($"Error getting categories: {ex.Message}"));
            }
        }

        // GET: api/category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var response = await _supabase.Client
                    .From<Category>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                var category = response.Models?.FirstOrDefault();
                if (category == null)
                    return NotFound(ApiResponse.ErrorResult("Category not found"));

                var result = new { category.Id, category.Name };
                return Ok(ApiResponse<object>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error getting category: {ex.Message}"));
            }
        }

        // POST: api/category
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CategoryDto categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = categoryDto.Name
                };

                var response = await _supabase.Client.From<Category>().Insert(category);
                var created = response.Models?.FirstOrDefault();

                if (created == null)
                    return BadRequest(ApiResponse.ErrorResult("Failed to create category"));

                var result = new { created.Id, created.Name };
                return Ok(ApiResponse<object>.SuccessResult(result, "Category created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return BadRequest(ApiResponse.ErrorResult($"Error creating category: {ex.Message}"));
            }
        }

        // PUT: api/category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryDto categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResult("Invalid input data"));

                var updated = new Category
                {
                    Id = id,
                    Name = categoryDto.Name
                };

                var response = await _supabase.Client.From<Category>().Update(updated);
                var category = response.Models?.FirstOrDefault();

                if (category == null)
                    return NotFound(ApiResponse.ErrorResult("Category not found or update failed"));

                var result = new { category.Id, category.Name };
                return Ok(ApiResponse<object>.SuccessResult(result, "Category updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error updating category: {ex.Message}"));
            }
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var item = new Category { Id = id };
                await _supabase.Client.From<Category>().Delete(item);

                return Ok(ApiResponse.SuccessResult("Category deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {Id}", id);
                return BadRequest(ApiResponse.ErrorResult($"Error deleting category: {ex.Message}"));
            }
        }
    }
}
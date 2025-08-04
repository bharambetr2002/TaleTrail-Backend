using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public CategoryController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        // GET: api/category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _supabase.Client.From<Category>().Get();
            return Ok(response.Models);
        }

        // GET: api/category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _supabase.Client
                .From<Category>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            return Ok(response.Models);
        }

        // POST: api/category
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Category category)
        {
            category.Id = Guid.NewGuid();
            var response = await _supabase.Client.From<Category>().Insert(category);
            return Ok(response.Models);
        }

        // PUT: api/category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Category updated)
        {
            updated.Id = id;
            var response = await _supabase.Client.From<Category>().Update(updated);
            return Ok(response.Models);
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = new Category { Id = id };
            var response = await _supabase.Client.From<Category>().Delete(item);
            return Ok(response.Models);
        }
    }
}
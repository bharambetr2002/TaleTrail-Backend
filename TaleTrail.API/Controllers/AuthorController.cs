using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.Models;
using System;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public AuthorController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        // GET: api/author
        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            var response = await _supabase.Client.From<Author>().Get();
            return Ok(response.Models);
        }

        // GET: api/author/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            var response = await _supabase.Client
                .From<Author>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var author = response.Models.FirstOrDefault();
            if (author == null) return NotFound();

            return Ok(author);
        }

        // POST: api/author
        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromBody] Author author)
        {
            author.Id = Guid.NewGuid();
            var response = await _supabase.Client.From<Author>().Insert(author);
            return Ok(response.Models);
        }

        // PUT: api/author/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(Guid id, [FromBody] Author updatedAuthor)
        {
            updatedAuthor.Id = id;
            var response = await _supabase.Client.From<Author>().Update(updatedAuthor);
            return Ok(response.Models);
        }

        // DELETE: api/author/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            var author = new Author { Id = id };
            var response = await _supabase.Client.From<Author>().Delete(author);
            return Ok(response.Models);
        }
    }
}
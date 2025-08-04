using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public BlogController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        // GET: api/blog
        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            var response = await _supabase.Client.From<Blog>().Get();
            return Ok(response.Models);
        }

        // GET: api/blog/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserBlogs(Guid userId)
        {
            var response = await _supabase.Client
                .From<Blog>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();

            return Ok(response.Models);
        }

        // POST: api/blog
        [HttpPost]
        public async Task<IActionResult> AddBlog([FromBody] Blog blog)
        {
            blog.Id = Guid.NewGuid();
            blog.CreatedAt = DateTime.UtcNow;

            var response = await _supabase.Client.From<Blog>().Insert(blog);
            return Ok(response.Models);
        }

        // PUT: api/blog/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(Guid id, [FromBody] Blog updatedBlog)
        {
            updatedBlog.Id = id;

            var response = await _supabase.Client.From<Blog>().Update(updatedBlog);
            return Ok(response.Models);
        }

        // DELETE: api/blog/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(Guid id)
        {
            var blog = new Blog { Id = id };
            var response = await _supabase.Client.From<Blog>().Delete(blog);
            return Ok(response.Models);
        }
    }
}
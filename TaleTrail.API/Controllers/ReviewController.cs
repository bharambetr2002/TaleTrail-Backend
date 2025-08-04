using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public ReviewController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        // GET: api/review
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _supabase.Client.From<Review>().Get();
            return Ok(response.Models);
        }

        // GET: api/review/book/{bookId}
        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetByBook(Guid bookId)
        {
            var response = await _supabase.Client
                .From<Review>()
                .Filter("book_id", Supabase.Postgrest.Constants.Operator.Equals, bookId.ToString())
                .Get();

            return Ok(response.Models);
        }

        // GET: api/review/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var response = await _supabase.Client
                .From<Review>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();

            return Ok(response.Models);
        }

        // POST: api/review
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Review review)
        {
            review.Id = Guid.NewGuid();
            review.CreatedAt = DateTime.UtcNow;

            var response = await _supabase.Client.From<Review>().Insert(review);
            return Ok(response.Models);
        }

        // PUT: api/review/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Review updated)
        {
            updated.Id = id;

            var response = await _supabase.Client.From<Review>().Update(updated);
            return Ok(response.Models);
        }

        // DELETE: api/review/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var review = new Review { Id = id };
            var response = await _supabase.Client.From<Review>().Delete(review);
            return Ok(response.Models);
        }
    }
}
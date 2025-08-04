using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public FeedbackController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        // GET: api/feedback
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _supabase.Client.From<Feedback>().Get();
            return Ok(response.Models);
        }

        // GET: api/feedback/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var response = await _supabase.Client
                .From<Feedback>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();

            return Ok(response.Models);
        }

        // POST: api/feedback
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Feedback feedback)
        {
            feedback.Id = Guid.NewGuid();
            feedback.CreatedAt = DateTime.UtcNow;

            var response = await _supabase.Client.From<Feedback>().Insert(feedback);
            return Ok(response.Models);
        }

        // DELETE: api/feedback/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = new Feedback { Id = id };
            var response = await _supabase.Client.From<Feedback>().Delete(item);
            return Ok(response.Models);
        }
    }
}
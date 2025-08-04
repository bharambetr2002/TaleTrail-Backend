using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public SubscriptionController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _supabase.Client.From<Subscription>().Get();
            return Ok(response.Models);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var response = await _supabase.Client
                .From<Subscription>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();

            return Ok(response.Models);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Subscription subscription)
        {
            subscription.Id = Guid.NewGuid();
            subscription.IsActive = true;
            var response = await _supabase.Client.From<Subscription>().Insert(subscription);
            return Ok(response.Models);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Subscription subscription)
        {
            subscription.Id = id;
            var response = await _supabase.Client.From<Subscription>().Update(subscription);
            return Ok(response.Models);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var subscription = new Subscription { Id = id };
            var response = await _supabase.Client.From<Subscription>().Delete(subscription);
            return Ok(response.Models);
        }
    }
}
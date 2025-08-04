using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WatchlistController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public WatchlistController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserWatchlist(Guid userId)
        {
            var response = await _supabase.Client
                .From<Watchlist>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();

            return Ok(response.Models);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWatchlist([FromBody] Watchlist watchlist)
        {
            watchlist.Id = Guid.NewGuid();
            watchlist.AddedAt = DateTime.UtcNow;

            var response = await _supabase.Client.From<Watchlist>().Insert(watchlist);
            return Ok(response.Models);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] Watchlist updated)
        {
            updated.Id = id;
            var response = await _supabase.Client.From<Watchlist>().Update(updated);
            return Ok(response.Models);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromWatchlist(Guid id)
        {
            var record = new Watchlist { Id = id };
            var response = await _supabase.Client.From<Watchlist>().Delete(record);
            return Ok(response.Models);
        }
    }
}
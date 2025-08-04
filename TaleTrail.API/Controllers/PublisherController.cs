using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublisherController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public PublisherController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        // GET: api/publisher
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _supabase.Client.From<Publisher>().Get();
            return Ok(response.Models);
        }

        // GET: api/publisher/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _supabase.Client
                .From<Publisher>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            return Ok(response.Models);
        }

        // POST: api/publisher
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Publisher publisher)
        {
            publisher.Id = Guid.NewGuid();
            var response = await _supabase.Client.From<Publisher>().Insert(publisher);
            return Ok(response.Models);
        }

        // PUT: api/publisher/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Publisher updated)
        {
            updated.Id = id;
            var response = await _supabase.Client.From<Publisher>().Update(updated);
            return Ok(response.Models);
        }

        // DELETE: api/publisher/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var record = new Publisher { Id = id };
            var response = await _supabase.Client.From<Publisher>().Delete(record);
            return Ok(response.Models);
        }
    }
}
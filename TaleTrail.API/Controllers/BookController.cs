using Microsoft.AspNetCore.Mvc;
using TaleTrail.API.Services;
using TaleTrail.API.Models;
using System;
using System.Threading.Tasks;

namespace TaleTrail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public BookController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        // GET: api/book
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var response = await _supabase.Client.From<Book>().Get();
            return Ok(response.Models);
        }

        // GET: api/book/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var response = await _supabase.Client
                .From<Book>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            return Ok(response.Models);
        }

        // POST: api/book
        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            book.Id = Guid.NewGuid();
            book.CreatedAt = DateTime.UtcNow;

            var response = await _supabase.Client.From<Book>().Insert(book);
            return Ok(response.Models);
        }

        // PUT: api/book/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] Book updatedBook)
        {
            updatedBook.Id = id;

            var response = await _supabase.Client.From<Book>().Update(updatedBook);
            return Ok(response.Models);
        }

        // DELETE: api/book/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var book = new Book { Id = id };
            var response = await _supabase.Client.From<Book>().Delete(book);
            return Ok(response.Models);
        }
    }
}
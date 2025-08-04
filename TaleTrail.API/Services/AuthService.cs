using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Services
{
    public class BookService
    {
        private readonly SupabaseService _supabase;

        public BookService(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            var response = await _supabase.Client.From<Book>().Get();
            return response.Models;
        }

        public async Task<Book> GetBookByIdAsync(Guid id)
        {
            var response = await _supabase.Client.From<Book>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var book = response.Models.FirstOrDefault();
            if (book == null)
                throw new NotFoundException($"Book with ID {id} not found");

            return book;
        }

        public async Task<Book> CreateBookAsync(BookDto bookDto, Guid userId)
        {
            ValidationHelper.ValidateModel(bookDto);

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = bookDto.Title,
                Description = bookDto.Description,
                CoverUrl = bookDto.CoverUrl,
                Language = bookDto.Language,
                PublicationYear = bookDto.PublicationYear,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var response = await _supabase.Client.From<Book>().Insert(book);
            return response.Models.First();
        }

        public async Task<Book> UpdateBookAsync(Guid id, BookDto bookDto)
        {
            ValidationHelper.ValidateModel(bookDto);

            var existingBook = await GetBookByIdAsync(id);

            existingBook.Title = bookDto.Title;
            existingBook.Description = bookDto.Description;
            existingBook.CoverUrl = bookDto.CoverUrl;
            existingBook.Language = bookDto.Language;
            existingBook.PublicationYear = bookDto.PublicationYear;

            var response = await _supabase.Client.From<Book>().Update(existingBook);
            return response.Models.First();
        }

        public async Task DeleteBookAsync(Guid id)
        {
            var book = await GetBookByIdAsync(id);
            await _supabase.Client.From<Book>().Delete(book);
        }
    }
}
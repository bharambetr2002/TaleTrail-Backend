using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Services
{
    public class BookService
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<BookService> _logger;

        public BookService(SupabaseService supabase, ILogger<BookService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<List<BookResponseDto>> GetAllBooksAsync()
        {
            var response = await _supabase.Client.From<Book>()
                .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            return response.Models?.ToDto() ?? new List<BookResponseDto>();
        }

        public async Task<List<BookResponseDto>> GetUserBooksAsync(Guid userId)
        {
            var response = await _supabase.Client.From<Book>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            return response.Models?.ToDto() ?? new List<BookResponseDto>();
        }

        public async Task<BookResponseDto> GetBookByIdAsync(Guid id)
        {
            var response = await _supabase.Client.From<Book>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var book = response.Models?.FirstOrDefault();
            if (book == null)
                throw new NotFoundException($"Book with ID {id} not found");

            return book.ToDto();
        }

        public async Task<BookResponseDto> CreateBookAsync(BookDto bookDto, Guid userId)
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
                UserId = userId, // From JWT token, not from client
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var response = await _supabase.Client.From<Book>().Insert(book);
                var createdBook = response.Models?.FirstOrDefault();

                if (createdBook == null)
                    throw new AppException("Failed to create book - no data returned");

                _logger.LogInformation("Book created successfully with ID {BookId} for user {UserId}", createdBook.Id, userId);
                return createdBook.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create book for user {UserId}. Title: {Title}", userId, bookDto.Title);
                throw;
            }
        }

        public async Task<BookResponseDto> UpdateBookAsync(Guid id, BookDto bookDto, Guid userId)
        {
            ValidationHelper.ValidateModel(bookDto);

            var existingBook = await GetBookByIdForUser(id, userId);

            existingBook.Title = bookDto.Title;
            existingBook.Description = bookDto.Description;
            existingBook.CoverUrl = bookDto.CoverUrl;
            existingBook.Language = bookDto.Language;
            existingBook.PublicationYear = bookDto.PublicationYear;

            try
            {
                var response = await _supabase.Client.From<Book>().Update(existingBook);
                var updatedBook = response.Models?.FirstOrDefault();

                if (updatedBook == null)
                    throw new AppException("Failed to update book");

                _logger.LogInformation("Book {BookId} updated successfully by user {UserId}", id, userId);
                return updatedBook.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update book {BookId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task DeleteBookAsync(Guid id, Guid userId)
        {
            var book = await GetBookByIdForUser(id, userId);

            try
            {
                await _supabase.Client.From<Book>().Delete(book);
                _logger.LogInformation("Book {BookId} deleted successfully by user {UserId}", id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete book {BookId} for user {UserId}", id, userId);
                throw new AppException($"Failed to delete book: {ex.Message}", ex);
            }
        }

        private async Task<Book> GetBookByIdForUser(Guid id, Guid userId)
        {
            var response = await _supabase.Client.From<Book>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var book = response.Models?.FirstOrDefault();
            if (book == null)
                throw new NotFoundException($"Book with ID {id} not found");

            // Authorization check: Only allow the owner to modify/delete
            if (book.UserId != userId)
                throw new UnauthorizedAccessException("You can only modify your own books");

            return book;
        }
    }
}
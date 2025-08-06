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
            var response = await _supabase.Client.From<Book>().Get();
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

            // Validate that the user exists
            await ValidateUserExistsAsync(userId);

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

        public async Task<BookResponseDto> UpdateBookAsync(Guid id, BookDto bookDto)
        {
            ValidationHelper.ValidateModel(bookDto);

            var existingBookResponse = await _supabase.Client.From<Book>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var existingBook = existingBookResponse.Models?.FirstOrDefault();
            if (existingBook == null)
                throw new NotFoundException($"Book with ID {id} not found");

            existingBook.Title = bookDto.Title;
            existingBook.Description = bookDto.Description;
            existingBook.CoverUrl = bookDto.CoverUrl;
            existingBook.Language = bookDto.Language;
            existingBook.PublicationYear = bookDto.PublicationYear;

            var response = await _supabase.Client.From<Book>().Update(existingBook);
            var updatedBook = response.Models?.FirstOrDefault();

            if (updatedBook == null)
                throw new AppException("Failed to update book");

            return updatedBook.ToDto();
        }

        public async Task DeleteBookAsync(Guid id)
        {
            var response = await _supabase.Client.From<Book>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var book = response.Models?.FirstOrDefault();
            if (book == null)
                throw new NotFoundException($"Book with ID {id} not found");

            await _supabase.Client.From<Book>().Delete(book);
            _logger.LogInformation("Book with ID {BookId} deleted successfully", id);
        }

        private async Task ValidateUserExistsAsync(Guid userId)
        {
            try
            {
                var userResponse = await _supabase.Client.From<User>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Get();

                if (!userResponse.Models?.Any() == true)
                {
                    throw new NotFoundException($"User with ID {userId} does not exist. Please ensure the user is registered before creating books.");
                }

                _logger.LogDebug("User validation successful for userId: {UserId}", userId);
            }
            catch (NotFoundException)
            {
                throw; // Re-throw NotFoundException as-is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user existence for userId: {UserId}", userId);
                throw new AppException($"Failed to validate user: {ex.Message}", ex);
            }
        }
    }
}
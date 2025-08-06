using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Services
{
    public class AuthorService
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<AuthorService> _logger;

        public AuthorService(SupabaseService supabase, ILogger<AuthorService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            try
            {
                var response = await _supabase.Client.From<Author>()
                    .Order("name", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                return response.Models ?? new List<Author>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all authors");
                throw new AppException($"Failed to get authors: {ex.Message}", ex);
            }
        }

        public async Task<Author?> GetAuthorByIdAsync(Guid id)
        {
            try
            {
                var response = await _supabase.Client.From<Author>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                return response.Models?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get author {AuthorId}", id);
                throw new AppException($"Failed to get author: {ex.Message}", ex);
            }
        }

        public async Task<Author> CreateAuthorAsync(AuthorDto authorDto)
        {
            ValidationHelper.ValidateModel(authorDto);

            var author = new Author
            {
                Id = Guid.NewGuid(),
                Name = authorDto.Name
            };

            try
            {
                var response = await _supabase.Client.From<Author>().Insert(author);
                var createdAuthor = response.Models?.FirstOrDefault();

                if (createdAuthor == null)
                    throw new AppException("Failed to create author - no data returned");

                _logger.LogInformation("Author created successfully with ID {AuthorId}", createdAuthor.Id);
                return createdAuthor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create author with name {Name}", authorDto.Name);
                throw;
            }
        }
    }
}
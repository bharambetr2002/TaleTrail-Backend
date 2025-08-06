using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Services
{
    public class BlogService
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<BlogService> _logger;

        public BlogService(SupabaseService supabase, ILogger<BlogService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<List<BlogResponseDto>> GetAllBlogsAsync()
        {
            var response = await _supabase.Client.From<Blog>()
                .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            return response.Models.ToDto();
        }

        public async Task<List<BlogResponseDto>> GetUserBlogsAsync(Guid userId)
        {
            var response = await _supabase.Client.From<Blog>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            return response.Models.ToDto();
        }

        public async Task<BlogResponseDto> CreateBlogAsync(BlogDto blogDto, Guid userId)
        {
            ValidationHelper.ValidateModel(blogDto);

            // Validate that the user exists
            await ValidateUserExistsAsync(userId);

            var blog = new Blog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = blogDto.Title,
                Content = blogDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var response = await _supabase.Client.From<Blog>().Insert(blog);
                var createdBlog = response.Models?.FirstOrDefault();

                if (createdBlog == null)
                    throw new AppException("Failed to create blog - no data returned");

                _logger.LogInformation("Blog created successfully with ID {BlogId} for user {UserId}", createdBlog.Id, userId);

                return createdBlog.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create blog for user {UserId}. Title: {Title}", userId, blogDto.Title);
                throw;
            }
        }

        public async Task<BlogResponseDto> UpdateBlogAsync(Guid id, BlogDto blogDto, Guid userId)
        {
            ValidationHelper.ValidateModel(blogDto);

            var existingBlog = await GetBlogByIdAsync(id);

            if (existingBlog.UserId != userId)
                throw new AppException("You can only update your own blogs");

            existingBlog.Title = blogDto.Title;
            existingBlog.Content = blogDto.Content;

            var response = await _supabase.Client.From<Blog>().Update(existingBlog);
            var updatedBlog = response.Models?.FirstOrDefault();

            if (updatedBlog == null)
                throw new AppException("Failed to update blog");

            return updatedBlog.ToDto();
        }

        private async Task<Blog> GetBlogByIdAsync(Guid id)
        {
            var response = await _supabase.Client.From<Blog>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var blog = response.Models?.FirstOrDefault();
            if (blog == null)
                throw new NotFoundException($"Blog with ID {id} not found");

            return blog;
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
                    throw new NotFoundException($"User with ID {userId} does not exist. Please ensure the user is registered before creating blogs.");
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
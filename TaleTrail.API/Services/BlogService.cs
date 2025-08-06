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
            try
            {
                var response = await _supabase.Client.From<Blog>()
                    .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                return response.Models?.ToDto() ?? new List<BlogResponseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all blogs");
                throw new AppException($"Failed to get blogs: {ex.Message}", ex);
            }
        }

        public async Task<List<BlogResponseDto>> GetUserBlogsAsync(Guid userId)
        {
            try
            {
                var response = await _supabase.Client.From<Blog>()
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                return response.Models?.ToDto() ?? new List<BlogResponseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get blogs for user {UserId}", userId);
                throw new AppException($"Failed to get user blogs: {ex.Message}", ex);
            }
        }

        public async Task<BlogResponseDto> CreateBlogAsync(BlogDto blogDto, Guid userId)
        {
            ValidationHelper.ValidateModel(blogDto);

            var blog = new Blog
            {
                Id = Guid.NewGuid(),
                UserId = userId, // From JWT token, not from client
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

            var existingBlog = await GetBlogByIdForUser(id, userId);

            existingBlog.Title = blogDto.Title;
            existingBlog.Content = blogDto.Content;

            try
            {
                var response = await _supabase.Client.From<Blog>().Update(existingBlog);
                var updatedBlog = response.Models?.FirstOrDefault();

                if (updatedBlog == null)
                    throw new AppException("Failed to update blog");

                _logger.LogInformation("Blog {BlogId} updated successfully by user {UserId}", id, userId);
                return updatedBlog.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update blog {BlogId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task DeleteBlogAsync(Guid id, Guid userId)
        {
            var blog = await GetBlogByIdForUser(id, userId);

            try
            {
                await _supabase.Client.From<Blog>().Delete(blog);
                _logger.LogInformation("Blog {BlogId} deleted successfully by user {UserId}", id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete blog {BlogId} for user {UserId}", id, userId);
                throw new AppException($"Failed to delete blog: {ex.Message}", ex);
            }
        }

        private async Task<Blog> GetBlogByIdForUser(Guid id, Guid userId)
        {
            var response = await _supabase.Client.From<Blog>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var blog = response.Models?.FirstOrDefault();
            if (blog == null)
                throw new NotFoundException($"Blog with ID {id} not found");

            // Authorization check: Only allow the owner to modify/delete
            if (blog.UserId != userId)
                throw new UnauthorizedAccessException("You can only modify your own blogs");

            return blog;
        }
    }
}
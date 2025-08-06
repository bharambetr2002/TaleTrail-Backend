// TaleTrail.API/Services/BlogLikeService.cs
using TaleTrail.API.Models;
using TaleTrail.API.Exceptions;

namespace TaleTrail.API.Services
{
    public class BlogLikeService
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<BlogLikeService> _logger;

        public BlogLikeService(SupabaseService supabase, ILogger<BlogLikeService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<List<BlogLike>> GetBlogLikesAsync(Guid blogId)
        {
            try
            {
                var response = await _supabase.Client.From<BlogLike>()
                    .Filter("blog_id", Supabase.Postgrest.Constants.Operator.Equals, blogId.ToString())
                    .Get();

                return response.Models ?? new List<BlogLike>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get likes for blog {BlogId}", blogId);
                throw new AppException($"Failed to get blog likes: {ex.Message}", ex);
            }
        }

        public async Task<int> GetBlogLikesCountAsync(Guid blogId)
        {
            var likes = await GetBlogLikesAsync(blogId);
            return likes.Count;
        }

        public async Task<bool> HasUserLikedBlogAsync(Guid blogId, Guid userId)
        {
            try
            {
                var response = await _supabase.Client.From<BlogLike>()
                    .Filter("blog_id", Supabase.Postgrest.Constants.Operator.Equals, blogId.ToString())
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Get();

                return response.Models?.Any() ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check if user {UserId} liked blog {BlogId}", userId, blogId);
                return false;
            }
        }

        public async Task<BlogLike> LikeBlogAsync(Guid blogId, Guid userId)
        {
            // Check if already liked
            var hasLiked = await HasUserLikedBlogAsync(blogId, userId);
            if (hasLiked)
                throw new AppException("You have already liked this blog");

            var blogLike = new BlogLike
            {
                Id = Guid.NewGuid(),
                BlogId = blogId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var response = await _supabase.Client.From<BlogLike>().Insert(blogLike);
                var createdLike = response.Models?.FirstOrDefault();

                if (createdLike == null)
                    throw new AppException("Failed to like blog - no data returned");

                _logger.LogInformation("Blog {BlogId} liked by user {UserId}", blogId, userId);
                return createdLike;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to like blog {BlogId} by user {UserId}", blogId, userId);
                throw;
            }
        }

        public async Task UnlikeBlogAsync(Guid blogId, Guid userId)
        {
            try
            {
                var response = await _supabase.Client.From<BlogLike>()
                    .Filter("blog_id", Supabase.Postgrest.Constants.Operator.Equals, blogId.ToString())
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Get();

                var blogLike = response.Models?.FirstOrDefault();
                if (blogLike == null)
                    throw new NotFoundException("Like not found");

                await _supabase.Client.From<BlogLike>().Delete(blogLike);
                _logger.LogInformation("Blog {BlogId} unliked by user {UserId}", blogId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unlike blog {BlogId} by user {UserId}", blogId, userId);
                throw new AppException($"Failed to unlike blog: {ex.Message}", ex);
            }
        }
    }
}
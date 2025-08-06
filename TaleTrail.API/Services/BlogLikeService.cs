using TaleTrail.API.DAO;
using TaleTrail.API.Models;
using TaleTrail.API.Exceptions;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.Services
{
    public class BlogLikeService
    {
        private readonly BlogLikeDao _blogLikeDao;
        private readonly ILogger<BlogLikeService> _logger;

        public BlogLikeService(BlogLikeDao blogLikeDao, ILogger<BlogLikeService> logger)
        {
            _blogLikeDao = blogLikeDao;
            _logger = logger;
        }

        public async Task<bool> HasUserLikedBlogAsync(Guid blogId, Guid userId)
        {
            var existingLike = await _blogLikeDao.GetByBlogAndUser(blogId, userId);
            return existingLike != null;
        }

        public async Task<BlogLike> LikeBlogAsync(Guid blogId, Guid userId)
        {
            if (await HasUserLikedBlogAsync(blogId, userId))
            {
                throw new AppException("You have already liked this blog.");
            }

            var blogLike = new BlogLike
            {
                BlogId = blogId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var createdLike = await _blogLikeDao.Add(blogLike);
            if (createdLike == null)
            {
                throw new AppException("Failed to like the blog.");
            }

            _logger.LogInformation("Blog {BlogId} liked by user {UserId}", blogId, userId);
            return createdLike;
        }

        public async Task<bool> UnlikeBlogAsync(Guid blogId, Guid userId)
        {
            var existingLike = await _blogLikeDao.GetByBlogAndUser(blogId, userId);
            if (existingLike == null)
            {
                return false; // Like not found
            }

            await _blogLikeDao.Delete(existingLike);
            _logger.LogInformation("Blog {BlogId} unliked by user {UserId}", blogId, userId);
            return true;
        }
    }
}

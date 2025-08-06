using TaleTrail.API.DAO;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.Services
{
    public class BlogService
    {
        private readonly BlogDao _blogDao;
        private readonly ILogger<BlogService> _logger;

        public BlogService(BlogDao blogDao, ILogger<BlogService> logger)
        {
            _blogDao = blogDao;
            _logger = logger;
        }

        public async Task<List<Blog>> GetBlogsAsync(Guid? userId = null)
        {
            return await _blogDao.GetAllAsync(userId);
        }

        public async Task<Blog> CreateBlogAsync(BlogDto blogDto, Guid userId)
        {
            var blog = new Blog
            {
                UserId = userId,
                Title = blogDto.Title,
                Content = blogDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            var createdBlog = await _blogDao.AddAsync(blog);
            if (createdBlog == null)
            {
                throw new AppException("Failed to create blog.");
            }
            return createdBlog;
        }

        public async Task<Blog?> UpdateBlogAsync(Guid id, BlogDto blogDto, Guid userId)
        {
            var existingBlog = await _blogDao.GetByIdAsync(id);
            if (existingBlog == null)
            {
                return null; // Not found
            }

            // Authorization check: Only allow the owner to update
            if (existingBlog.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to update blog {BlogId} owned by {OwnerId}", userId, id, existingBlog.UserId);
                return null;
            }

            existingBlog.Title = blogDto.Title;
            existingBlog.Content = blogDto.Content;

            return await _blogDao.UpdateAsync(existingBlog);
        }

        public async Task<bool> DeleteBlogAsync(Guid id, Guid userId)
        {
            var existingBlog = await _blogDao.GetByIdAsync(id);
            if (existingBlog == null)
            {
                return false; // Not found
            }

            // Authorization check: Only allow the owner to delete
            if (existingBlog.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to delete blog {BlogId} owned by {OwnerId}", userId, id, existingBlog.UserId);
                return false;
            }

            await _blogDao.DeleteAsync(id);
            return true;
        }
    }
}

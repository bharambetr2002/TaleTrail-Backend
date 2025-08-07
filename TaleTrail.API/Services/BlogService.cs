using TaleTrail.API.DAO;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaleTrail.API.Services
{
    public class BlogService
    {
        private readonly BlogDao _blogDao;

        public BlogService(BlogDao blogDao)
        {
            _blogDao = blogDao;
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
            if (existingBlog == null || existingBlog.UserId != userId)
            {
                return null;
            }

            existingBlog.Title = blogDto.Title;
            existingBlog.Content = blogDto.Content;
            return await _blogDao.UpdateAsync(existingBlog);
        }

        public async Task<bool> DeleteBlogAsync(Guid id, Guid userId)
        {
            var existingBlog = await _blogDao.GetByIdAsync(id);
            if (existingBlog == null || existingBlog.UserId != userId)
            {
                return false;
            }
            await _blogDao.DeleteAsync(id);
            return true;
        }
    }
}
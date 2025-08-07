using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.DAO;

namespace TaleTrail.API.Services;

public class BlogService
{
    private readonly BlogDao _blogDao;
    private readonly BlogLikeDao _blogLikeDao;

    public BlogService(BlogDao blogDao, BlogLikeDao blogLikeDao)
    {
        _blogDao = blogDao;
        _blogLikeDao = blogLikeDao;
    }

    public async Task<List<Blog>> GetAllBlogsAsync()
    {
        return await _blogDao.GetAllAsync();
    }

    public async Task<List<Blog>> GetBlogsByUserIdAsync(Guid userId)
    {
        return await _blogDao.GetByUserIdAsync(userId);
    }

    public async Task<Blog?> GetBlogByIdAsync(Guid id)
    {
        return await _blogDao.GetByIdAsync(id);
    }

    public async Task<Blog> CreateBlogAsync(Guid userId, CreateBlogRequest request)
    {
        var blog = new Blog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _blogDao.CreateAsync(blog);
    }

    public async Task<Blog> UpdateBlogAsync(Guid blogId, Guid userId, UpdateBlogRequest request)
    {
        var existingBlog = await _blogDao.GetByIdAsync(blogId);
        if (existingBlog == null)
            throw new Exception("Blog not found");

        if (existingBlog.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own blogs");

        existingBlog.Title = request.Title;
        existingBlog.Content = request.Content;

        return await _blogDao.UpdateAsync(existingBlog);
    }

    public async Task DeleteBlogAsync(Guid blogId, Guid userId)
    {
        var existingBlog = await _blogDao.GetByIdAsync(blogId);
        if (existingBlog == null)
            throw new Exception("Blog not found");

        if (existingBlog.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own blogs");

        await _blogDao.DeleteAsync(blogId);
    }

    public async Task<BlogLike> LikeBlogAsync(Guid blogId, Guid userId)
    {
        // Check if already liked
        var existingLike = await _blogLikeDao.GetByBlogAndUserAsync(blogId, userId);
        if (existingLike != null)
            throw new Exception("Blog already liked");

        var blogLike = new BlogLike
        {
            Id = Guid.NewGuid(),
            BlogId = blogId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        return await _blogLikeDao.CreateAsync(blogLike);
    }

    public async Task UnlikeBlogAsync(Guid blogId, Guid userId)
    {
        var existingLike = await _blogLikeDao.GetByBlogAndUserAsync(blogId, userId);
        if (existingLike == null)
            throw new Exception("Blog not liked");

        await _blogLikeDao.DeleteAsync(blogId, userId);
    }

    public async Task<int> GetLikeCountAsync(Guid blogId)
    {
        return await _blogLikeDao.GetLikeCountAsync(blogId);
    }
}

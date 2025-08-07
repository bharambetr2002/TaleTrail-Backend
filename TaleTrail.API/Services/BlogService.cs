using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.DAO;

namespace TaleTrail.API.Services;

public class BlogService
{
    private readonly BlogDao _blogDao;
    private readonly BlogLikeDao _blogLikeDao;
    private readonly UserDao _userDao;

    public BlogService(BlogDao blogDao, BlogLikeDao blogLikeDao, UserDao userDao)
    {
        _blogDao = blogDao;
        _blogLikeDao = blogLikeDao;
        _userDao = userDao;
    }

    public async Task<List<BlogResponseDTO>> GetAllBlogsAsync(Guid? currentUserId = null)
    {
        var blogs = await _blogDao.GetAllAsync();
        var blogDtos = new List<BlogResponseDTO>();

        foreach (var blog in blogs)
        {
            var blogDto = await MapToBlogResponseDTO(blog, currentUserId);
            blogDtos.Add(blogDto);
        }

        return blogDtos;
    }

    public async Task<List<BlogResponseDTO>> GetBlogsByUserIdAsync(Guid userId, Guid? currentUserId = null)
    {
        var blogs = await _blogDao.GetByUserIdAsync(userId);
        var blogDtos = new List<BlogResponseDTO>();

        foreach (var blog in blogs)
        {
            var blogDto = await MapToBlogResponseDTO(blog, currentUserId);
            blogDtos.Add(blogDto);
        }

        return blogDtos;
    }

    public async Task<BlogResponseDTO?> GetBlogByIdAsync(Guid id, Guid? currentUserId = null)
    {
        var blog = await _blogDao.GetByIdAsync(id);
        return blog == null ? null : await MapToBlogResponseDTO(blog, currentUserId);
    }

    public async Task<BlogResponseDTO> CreateBlogAsync(Guid userId, CreateBlogRequest request)
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

        var createdBlog = await _blogDao.CreateAsync(blog);
        return await MapToBlogResponseDTO(createdBlog, userId);
    }

    public async Task<BlogResponseDTO> UpdateBlogAsync(Guid blogId, Guid userId, UpdateBlogRequest request)
    {
        var existingBlog = await _blogDao.GetByIdAsync(blogId);
        if (existingBlog == null)
            throw new KeyNotFoundException("Blog not found");

        if (existingBlog.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own blogs");

        existingBlog.Title = request.Title;
        existingBlog.Content = request.Content;

        var updatedBlog = await _blogDao.UpdateAsync(existingBlog);
        return await MapToBlogResponseDTO(updatedBlog, userId);
    }

    public async Task DeleteBlogAsync(Guid blogId, Guid userId)
    {
        var existingBlog = await _blogDao.GetByIdAsync(blogId);
        if (existingBlog == null)
            throw new KeyNotFoundException("Blog not found");

        if (existingBlog.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own blogs");

        await _blogDao.DeleteAsync(blogId);
    }

    public async Task<BlogLike> LikeBlogAsync(Guid blogId, Guid userId)
    {
        // Check if already liked
        var existingLike = await _blogLikeDao.GetByBlogAndUserAsync(blogId, userId);
        if (existingLike != null)
            throw new InvalidOperationException("Blog already liked");

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
            throw new InvalidOperationException("Blog not liked");

        await _blogLikeDao.DeleteAsync(blogId, userId);
    }

    public async Task<int> GetLikeCountAsync(Guid blogId)
    {
        return await _blogLikeDao.GetLikeCountAsync(blogId);
    }

    private async Task<BlogResponseDTO> MapToBlogResponseDTO(Blog blog, Guid? currentUserId = null)
    {
        var user = await _userDao.GetByIdAsync(blog.UserId);
        var likeCount = await _blogLikeDao.GetLikeCountAsync(blog.Id);

        var isLikedByCurrentUser = false;
        if (currentUserId.HasValue)
        {
            var existingLike = await _blogLikeDao.GetByBlogAndUserAsync(blog.Id, currentUserId.Value);
            isLikedByCurrentUser = existingLike != null;
        }

        return new BlogResponseDTO
        {
            Id = blog.Id,
            UserId = blog.UserId,
            Username = user?.Username ?? "Unknown User",
            Title = blog.Title,
            Content = blog.Content,
            LikeCount = likeCount,
            IsLikedByCurrentUser = isLikedByCurrentUser,
            CreatedAt = blog.CreatedAt,
            UpdatedAt = blog.UpdatedAt
        };
    }
}
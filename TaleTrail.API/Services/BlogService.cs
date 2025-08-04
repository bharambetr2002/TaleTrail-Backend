using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Services
{
    public class BlogService
    {
        private readonly SupabaseService _supabase;

        public BlogService(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        public async Task<List<Blog>> GetAllBlogsAsync()
        {
            var response = await _supabase.Client.From<Blog>()
                .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();
            return response.Models;
        }

        public async Task<List<Blog>> GetUserBlogsAsync(Guid userId)
        {
            var response = await _supabase.Client.From<Blog>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();
            return response.Models;
        }

        public async Task<Blog> CreateBlogAsync(BlogDto blogDto, Guid userId)
        {
            ValidationHelper.ValidateModel(blogDto);

            var blog = new Blog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = blogDto.Title,
                Content = blogDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            var response = await _supabase.Client.From<Blog>().Insert(blog);
            return response.Models.First();
        }

        public async Task<Blog> UpdateBlogAsync(Guid id, BlogDto blogDto, Guid userId)
        {
            ValidationHelper.ValidateModel(blogDto);

            var existingBlog = await GetBlogByIdAsync(id);

            if (existingBlog.UserId != userId)
                throw new AppException("You can only update your own blogs");

            existingBlog.Title = blogDto.Title;
            existingBlog.Content = blogDto.Content;

            var response = await _supabase.Client.From<Blog>().Update(existingBlog);
            return response.Models.First();
        }

        private async Task<Blog> GetBlogByIdAsync(Guid id)
        {
            var response = await _supabase.Client.From<Blog>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var blog = response.Models.FirstOrDefault();
            if (blog == null)
                throw new NotFoundException($"Blog with ID {id} not found");

            return blog;
        }
    }
}

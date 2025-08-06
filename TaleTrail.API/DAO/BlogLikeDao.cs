// TaleTrail.API/DAO/BlogLikeDao.cs
using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;

namespace TaleTrail.API.DAO
{
    public class BlogLikeDao
    {
        private readonly Client _client;

        public BlogLikeDao(SupabaseService service) => _client = service.Client;

        public async Task<List<BlogLike>> GetByBlogId(Guid blogId) =>
            (await _client.From<BlogLike>().Where(bl => bl.BlogId == blogId).Get()).Models;

        public async Task<List<BlogLike>> GetByUserId(Guid userId) =>
            (await _client.From<BlogLike>().Where(bl => bl.UserId == userId).Get()).Models;

        public async Task<BlogLike?> GetByBlogAndUser(Guid blogId, Guid userId) =>
            (await _client.From<BlogLike>()
                .Where(bl => bl.BlogId == blogId && bl.UserId == userId)
                .Get()).Models?.FirstOrDefault();

        public async Task<BlogLike?> Add(BlogLike blogLike)
        {
            var response = await _client.From<BlogLike>().Insert(blogLike);
            return response.Models.FirstOrDefault();
        }

        public async Task Delete(BlogLike blogLike)
        {
            await _client.From<BlogLike>().Delete(blogLike);
        }
    }
}
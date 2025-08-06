using Supabase;
using TaleTrail.API.Models;
using TaleTrail.API.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TaleTrail.API.DAO
{
    public class BlogLikeDao
    {
        private readonly Client _client;

        public BlogLikeDao(SupabaseService service) => _client = service.Client;

        public async Task<List<BlogLike>> GetByBlogId(Guid blogId)
        {
            var response = await _client.From<BlogLike>()
                .Filter("blog_id", Supabase.Postgrest.Constants.Operator.Equals, blogId.ToString())
                .Get();
            return response.Models;
        }

        public async Task<List<BlogLike>> GetByUserId(Guid userId)
        {
            var response = await _client.From<BlogLike>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();
            return response.Models;
        }

        public async Task<BlogLike?> GetByBlogAndUser(Guid blogId, Guid userId)
        {
            var response = await _client.From<BlogLike>()
                .Filter("blog_id", Supabase.Postgrest.Constants.Operator.Equals, blogId.ToString())
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();
            return response.Models?.FirstOrDefault();
        }

        public async Task<BlogLike?> Add(BlogLike blogLike)
        {
            var response = await _client.From<BlogLike>().Insert(blogLike);
            return response.Models.FirstOrDefault();
        }

        public async Task Delete(Guid blogId, Guid userId)
        {
            await _client.From<BlogLike>()
                .Filter("blog_id", Supabase.Postgrest.Constants.Operator.Equals, blogId.ToString())
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Delete();
        }
    }
}
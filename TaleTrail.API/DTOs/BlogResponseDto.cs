namespace TaleTrail.API.DTOs
{
    public class BlogResponseDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public static class BlogExtensions
    {
        public static BlogResponseDto ToDto(this TaleTrail.API.Models.Blog blog)
        {
            return new BlogResponseDto
            {
                Id = blog.Id,
                UserId = blog.UserId,
                Title = blog.Title,
                Content = blog.Content,
                CreatedAt = blog.CreatedAt
            };
        }

        public static List<BlogResponseDto> ToDto(this List<TaleTrail.API.Models.Blog> blogs)
        {
            return blogs.Select(blog => blog.ToDto()).ToList();
        }
    }
}
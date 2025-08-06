using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace TaleTrail.API.Models
{
    [Table("blog_likes")]
    public class BlogLike : BaseModel
    {
        [PrimaryKey("blog_id", false)]
        [Column("blog_id")]
        public Guid BlogId { get; set; }

        [PrimaryKey("user_id", false)]
        [Column("user_id")]
        public Guid UserId { get; set; }

        // Note: created_at is not in the actual schema provided, removing it
        // If you need to track when likes were created, add it to your database schema
    }
}
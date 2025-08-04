using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace TaleTrail.API.Models;

[Table("blog_likes")]
public class BlogLike : BaseModel
{
    [PrimaryKey("blog_id", false)]
    [Column("blog_id")]
    public Guid BlogId { get; set; }

    [PrimaryKey("user_id", false)]
    [Column("user_id")]
    public Guid UserId { get; set; }
}
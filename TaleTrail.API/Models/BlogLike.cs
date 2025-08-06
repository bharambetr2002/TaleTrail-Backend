using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace TaleTrail.API.Models;

[Table("blog_likes")]
public class BlogLike : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("blog_id")]
    public Guid BlogId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
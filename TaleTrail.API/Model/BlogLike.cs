using Postgrest.Attributes;
using Postgrest.Models;

namespace TaleTrail.API.Model;

[Table("blog_likes")]
public class BlogLike : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("blog_id")]
    public Guid BlogId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

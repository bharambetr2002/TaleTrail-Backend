using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace TaleTrail.API.Models;

[Table("feedback")]
public class Feedback : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("message")]
    public string Message { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
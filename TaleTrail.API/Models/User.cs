using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace TaleTrail.API.Models;

[Table("users")]
public class User : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("full_name")]
    public string FullName { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("avatar_url")]
    public string? AvatarUrl { get; set; }

    [Column("bio")]
    public string? Bio { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
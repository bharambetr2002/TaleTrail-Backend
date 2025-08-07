using Postgrest.Attributes;
using Postgrest.Models;

namespace TaleTrail.API.Model;

[Table("users")]
public class User : BaseModel
{
    [PrimaryKey("id", false)]  // Supabase handles UUID
    public Guid Id { get; set; }

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("full_name")]
    public string? FullName { get; set; }

    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Column("bio")]
    public string? Bio { get; set; }

    [Column("avatar_url")]
    public string? AvatarUrl { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
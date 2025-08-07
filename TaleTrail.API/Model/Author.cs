using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace TaleTrail.API.Model;

[Table("authors")]
public class Author : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("bio")]
    public string? Bio { get; set; }

    [Column("birth_date")]
    public DateTime? BirthDate { get; set; }

    [Column("death_date")]
    public DateTime? DeathDate { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace TaleTrail.API.Models;

[Table("authors")]
public class Author : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;
}
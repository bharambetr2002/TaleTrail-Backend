using Postgrest.Attributes;
using Postgrest.Models;

namespace TaleTrail.API.Model;

[Table("publishers")]
public class Publisher : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("founded_year")]
    public int? FoundedYear { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
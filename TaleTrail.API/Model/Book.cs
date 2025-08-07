using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace TaleTrail.API.Model;

[Table("books")]
public class Book : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("cover_url")]
    public string? CoverUrl { get; set; }

    [Column("publication_year")]
    public int? PublicationYear { get; set; }

    [Column("publisher_id")]
    public Guid? PublisherId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
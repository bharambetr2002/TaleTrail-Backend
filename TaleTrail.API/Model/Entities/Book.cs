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

    [Column("language")]
    public string? Language { get; set; }

    [Column("cover_image_url")]
    public string? CoverImageUrl { get; set; }

    [Column("publication_year")]
    public int? PublicationYear { get; set; }

    [Column("author_id")]
    public Guid? AuthorId { get; set; }

    [Column("publisher_id")]
    public Guid? PublisherId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
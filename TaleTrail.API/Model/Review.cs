using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace TaleTrail.API.Model;

[Table("reviews")]
public class Review : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("book_id")]
    public Guid BookId { get; set; }

    [Column("rating")]
    public int Rating { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
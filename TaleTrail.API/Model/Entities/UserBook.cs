using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace TaleTrail.API.Model;

[Table("user_books")]
public class UserBook : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("book_id")]
    public Guid BookId { get; set; }

    [Column("reading_status")]
    public string ReadingStatus { get; set; } = "ToRead";

    [Column("progress")]
    public int Progress { get; set; } = 0;

    [Column("started_at")]
    public DateTime? StartedAt { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace TaleTrail.API.Model;

[Table("book_authors")]
public class BookAuthor : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("book_id")]
    public Guid BookId { get; set; }

    [Column("author_id")]
    public Guid AuthorId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
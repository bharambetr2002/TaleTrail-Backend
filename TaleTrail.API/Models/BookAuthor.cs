using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace TaleTrail.API.Models;

[Table("book_authors")]
public class BookAuthor : BaseModel
{
    [PrimaryKey("book_id", false)]
    [Column("book_id")]
    public Guid BookId { get; set; }

    [PrimaryKey("author_id", false)]
    [Column("author_id")]
    public Guid AuthorId { get; set; }
}
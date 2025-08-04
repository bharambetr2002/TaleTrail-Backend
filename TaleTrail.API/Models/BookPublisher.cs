using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace TaleTrail.API.Models;

[Table("book_publishers")]
public class BookPublisher : BaseModel
{
    [PrimaryKey("book_id", false)]
    [Column("book_id")]
    public Guid BookId { get; set; }

    [PrimaryKey("publisher_id", false)]
    [Column("publisher_id")]
    public Guid PublisherId { get; set; }
}
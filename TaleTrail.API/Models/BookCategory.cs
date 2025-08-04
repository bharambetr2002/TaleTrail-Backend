using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace TaleTrail.API.Models;

[Table("book_categories")]
public class BookCategory : BaseModel
{
    [PrimaryKey("book_id", false)]
    [Column("book_id")]
    public Guid BookId { get; set; }

    [PrimaryKey("category_id", false)]
    [Column("category_id")]
    public Guid CategoryId { get; set; }
}
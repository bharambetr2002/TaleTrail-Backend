using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System;

namespace TaleTrail.API.Models
{
    [Table("user_books")]
    public class UserBook : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("book_id")]
        public Guid BookId { get; set; }

        [Column("status")]
        public string Status { get; set; } = string.Empty;

        [Column("added_at")]
        public DateTime AddedAt { get; set; }

        [Column("started_at")]
        public DateTime? StartedAt { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }
    }
}
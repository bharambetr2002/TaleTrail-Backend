using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System;

namespace TaleTrail.API.Models
{
    [Table("blogs")]
    public class Blog : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; } // Changed to non-nullable

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("content")]
        public string? Content { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
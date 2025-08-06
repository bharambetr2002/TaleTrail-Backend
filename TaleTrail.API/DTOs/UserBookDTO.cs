using System.ComponentModel.DataAnnotations;
using System;

namespace TaleTrail.API.DTOs
{
    public class UserBookDTO
    {
        [Required]
        public Guid BookId { get; set; }

        [Required]
        [RegularExpression("^(wanna_read|in_progress|completed|dropped)$",
            ErrorMessage = "Status must be one of: wanna_read, in_progress, completed, dropped")]
        public string Status { get; set; } = string.Empty;
    }
}
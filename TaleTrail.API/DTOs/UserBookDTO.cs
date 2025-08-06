using System.ComponentModel.DataAnnotations;
using System;

namespace TaleTrail.API.DTOs
{
    public class UserBookDTO
    {
        [Required]
        public Guid BookId { get; set; }

        [Required]
        // You could create an Enum for this later for more type safety
        public string Status { get; set; } = string.Empty;
    }
}
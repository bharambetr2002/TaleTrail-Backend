using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace TaleTrail.API.DTOs
{
    public class BookDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? CoverUrl { get; set; }

        [Range(1000, 9999, ErrorMessage = "Publication year must be a valid year")]
        public int? PublicationYear { get; set; }

        [Required]
        public Guid PublisherId { get; set; }

        [Required]
        public List<Guid> AuthorIds { get; set; } = new List<Guid>();
    }
}
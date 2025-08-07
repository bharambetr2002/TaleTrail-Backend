using System;

namespace TaleTrail.API.DTOs
{
    public class BookResponseDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? PublicationYear { get; set; }
        // We can add author and publisher details here later if needed
    }
}
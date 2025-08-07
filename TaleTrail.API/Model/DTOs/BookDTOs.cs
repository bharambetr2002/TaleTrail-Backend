using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.Model.DTOs;

public class BookResponseDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Language { get; set; }
    public string? CoverImageUrl { get; set; }
    public int? PublicationYear { get; set; }
    public Guid? PublisherId { get; set; }
    public string? PublisherName { get; set; }
    public List<AuthorResponseDTO> Authors { get; set; } = new();
}

public class CreateBookRequestDTO
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Language { get; set; }

    [Range(1, 2024)]
    public int? PublicationYear { get; set; }

    public Guid? PublisherId { get; set; }

    public List<Guid> AuthorIds { get; set; } = new();
}
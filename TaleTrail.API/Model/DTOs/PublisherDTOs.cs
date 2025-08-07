using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.Model.DTOs;

public class PublisherResponseDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public int? FoundedYear { get; set; }
    public int BookCount { get; set; }
}

public class CreatePublisherRequestDTO
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    [Range(1800, 2024)]
    public int? FoundedYear { get; set; }
}
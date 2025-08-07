using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.Model.DTOs;

public class AuthorResponseDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public int BookCount { get; set; }
}

public class CreateAuthorRequestDTO
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Bio { get; set; }

    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}

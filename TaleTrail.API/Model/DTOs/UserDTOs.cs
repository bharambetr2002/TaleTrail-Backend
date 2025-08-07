using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.Model.DTOs;

public class UpdateUserRequestDTO
{
    [MaxLength(100)]
    public string? FullName { get; set; }

    [MaxLength(50)]
    public string? Username { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }

    [Url]
    public string? AvatarUrl { get; set; }
}
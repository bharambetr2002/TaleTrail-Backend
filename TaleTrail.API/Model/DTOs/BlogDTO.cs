using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.Model.DTOs;

/// <summary>
/// Request model for creating a new blog post
/// </summary>
public class CreateBlogRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(10000)]
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Request model for updating an existing blog post
/// </summary>
public class UpdateBlogRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(10000)]
    public string Content { get; set; } = string.Empty;
}
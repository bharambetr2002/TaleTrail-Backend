using System;
using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.DTOs
{
    public class AuthorDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
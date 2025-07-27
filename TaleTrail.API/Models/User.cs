using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? Role { get; set; } = "user";
    }
}
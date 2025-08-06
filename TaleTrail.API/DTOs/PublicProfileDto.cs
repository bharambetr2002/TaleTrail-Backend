using System;
using System.Collections.Generic;

namespace TaleTrail.API.DTOs.Profile
{
    public class PublicProfileDto
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Location { get; set; }
        public DateTime JoinedAt { get; set; }
        public UserStatsDto Stats { get; set; } = new UserStatsDto();

        // Using a simple object to avoid circular dependencies and shape the data cleanly.
        // This will hold a simplified list of books (e.g., Id, Title, CoverUrl).
        public List<object> CompletedBooks { get; set; } = new List<object>();
    }
}
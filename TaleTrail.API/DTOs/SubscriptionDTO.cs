using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.DTOs
{
    public class SubscriptionDto
    {
        [Required(ErrorMessage = "Plan name is required")]
        [StringLength(50, ErrorMessage = "Plan name cannot exceed 50 characters")]
        public string PlanName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
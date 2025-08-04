namespace TaleTrail.API.DTOs
{
    public class SubscriptionDto
    {
        public string PlanName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
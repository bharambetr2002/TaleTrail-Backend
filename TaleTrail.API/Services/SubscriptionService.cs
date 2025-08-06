using TaleTrail.API.Models;
using TaleTrail.API.DTOs;

namespace TaleTrail.API.Services
{
    public class SubscriptionService
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(SupabaseService supabase, ILogger<SubscriptionService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<List<Subscription>> GetAllAsync()
        {
            var response = await _supabase.Client.From<Subscription>().Get();
            return response.Models ?? new List<Subscription>();
        }

        public async Task<List<Subscription>> GetByUserIdAsync(Guid userId)
        {
            var response = await _supabase.Client.From<Subscription>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Order("start_date", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();
            return response.Models ?? new List<Subscription>();
        }

        public async Task<Subscription?> GetByIdAsync(Guid id)
        {
            var response = await _supabase.Client.From<Subscription>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();
            return response.Models?.FirstOrDefault();
        }

        public async Task<Subscription?> CreateAsync(SubscriptionDto dto, Guid userId)
        {
            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PlanName = dto.PlanName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true
            };
            var response = await _supabase.Client.From<Subscription>().Insert(subscription);
            return response.Models?.FirstOrDefault();
        }

        public async Task<Subscription?> UpdateAsync(Guid id, SubscriptionDto dto, Guid userId)
        {
            var response = await _supabase.Client.From<Subscription>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();
            var subscription = response.Models?.FirstOrDefault();
            if (subscription == null || subscription.UserId != userId) return null;
            subscription.PlanName = dto.PlanName;
            subscription.StartDate = dto.StartDate;
            subscription.EndDate = dto.EndDate;
            var updateResponse = await _supabase.Client.From<Subscription>().Update(subscription);
            return updateResponse.Models?.FirstOrDefault();
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var response = await _supabase.Client.From<Subscription>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();
            var subscription = response.Models?.FirstOrDefault();
            if (subscription == null || subscription.UserId != userId) return false;
            await _supabase.Client.From<Subscription>().Delete(subscription);
            return true;
        }
    }
}
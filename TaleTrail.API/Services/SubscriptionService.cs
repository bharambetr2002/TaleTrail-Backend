using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

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

        public async Task<List<Subscription>> GetAllSubscriptionsAsync()
        {
            try
            {
                var response = await _supabase.Client.From<Subscription>()
                    .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                return response.Models ?? new List<Subscription>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all subscriptions");
                throw new AppException($"Failed to get subscriptions: {ex.Message}", ex);
            }
        }

        public async Task<List<Subscription>> GetUserSubscriptionsAsync(Guid userId)
        {
            try
            {
                var response = await _supabase.Client.From<Subscription>()
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Order("start_date", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                return response.Models ?? new List<Subscription>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get subscriptions for user {UserId}", userId);
                throw new AppException($"Failed to get user subscriptions: {ex.Message}", ex);
            }
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(Guid id)
        {
            try
            {
                var response = await _supabase.Client.From<Subscription>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                return response.Models?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get subscription {SubscriptionId}", id);
                throw new AppException($"Failed to get subscription: {ex.Message}", ex);
            }
        }

        public async Task<Subscription> CreateSubscriptionAsync(SubscriptionDto subscriptionDto, Guid userId)
        {
            ValidationHelper.ValidateModel(subscriptionDto);

            // Check if user already has an active subscription
            var activeSubscriptions = await GetActiveUserSubscriptionsAsync(userId);
            if (activeSubscriptions.Any())
            {
                throw new AppException("User already has an active subscription");
            }

            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId, // From JWT token, not from client
                PlanName = subscriptionDto.PlanName,
                StartDate = subscriptionDto.StartDate,
                EndDate = subscriptionDto.EndDate,
                IsActive = true
            };

            try
            {
                var response = await _supabase.Client.From<Subscription>().Insert(subscription);
                var createdSubscription = response.Models?.FirstOrDefault();

                if (createdSubscription == null)
                    throw new AppException("Failed to create subscription - no data returned");

                _logger.LogInformation("Subscription created successfully with ID {SubscriptionId} for user {UserId}", createdSubscription.Id, userId);
                return createdSubscription;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create subscription for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Subscription> UpdateSubscriptionAsync(Guid id, SubscriptionDto subscriptionDto, Guid userId)
        {
            ValidationHelper.ValidateModel(subscriptionDto);

            var existingSubscription = await GetSubscriptionByIdForUser(id, userId);

            existingSubscription.PlanName = subscriptionDto.PlanName;
            existingSubscription.StartDate = subscriptionDto.StartDate;
            existingSubscription.EndDate = subscriptionDto.EndDate;

            try
            {
                var response = await _supabase.Client.From<Subscription>().Update(existingSubscription);
                var updatedSubscription = response.Models?.FirstOrDefault();

                if (updatedSubscription == null)
                    throw new AppException("Failed to update subscription");

                _logger.LogInformation("Subscription {SubscriptionId} updated successfully by user {UserId}", id, userId);
                return updatedSubscription;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update subscription {SubscriptionId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task CancelSubscriptionAsync(Guid id, Guid userId)
        {
            var subscription = await GetSubscriptionByIdForUser(id, userId);

            subscription.IsActive = false;
            subscription.EndDate = DateTime.UtcNow;

            try
            {
                var response = await _supabase.Client.From<Subscription>().Update(subscription);
                _logger.LogInformation("Subscription {SubscriptionId} cancelled successfully by user {UserId}", id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel subscription {SubscriptionId} for user {UserId}", id, userId);
                throw new AppException($"Failed to cancel subscription: {ex.Message}", ex);
            }
        }

        private async Task<List<Subscription>> GetActiveUserSubscriptionsAsync(Guid userId)
        {
            var response = await _supabase.Client.From<Subscription>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("is_active", Supabase.Postgrest.Constants.Operator.Equals, "true")
                .Get();

            return response.Models?.Where(s => s.EndDate == null || s.EndDate > DateTime.UtcNow).ToList() ?? new List<Subscription>();
        }

        private async Task<Subscription> GetSubscriptionByIdForUser(Guid id, Guid userId)
        {
            var response = await _supabase.Client.From<Subscription>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var subscription = response.Models?.FirstOrDefault();
            if (subscription == null)
                throw new NotFoundException($"Subscription with ID {id} not found");

            // Authorization check: Only allow the owner to modify
            if (subscription.UserId != userId)
                throw new UnauthorizedAccessException("You can only modify your own subscriptions");

            return subscription;
        }
    }
}
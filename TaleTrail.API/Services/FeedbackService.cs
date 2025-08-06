using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Services
{
    public class FeedbackService
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<FeedbackService> _logger;

        public FeedbackService(SupabaseService supabase, ILogger<FeedbackService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<List<Feedback>> GetAllFeedbackAsync()
        {
            try
            {
                var response = await _supabase.Client.From<Feedback>()
                    .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                return response.Models ?? new List<Feedback>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all feedback");
                throw new AppException($"Failed to get feedback: {ex.Message}", ex);
            }
        }

        public async Task<Feedback?> GetFeedbackByIdAsync(Guid id)
        {
            try
            {
                var response = await _supabase.Client.From<Feedback>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                return response.Models?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get feedback {FeedbackId}", id);
                throw new AppException($"Failed to get feedback: {ex.Message}", ex);
            }
        }

        public async Task<List<Feedback>> GetUserFeedbackAsync(Guid userId)
        {
            try
            {
                var response = await _supabase.Client.From<Feedback>()
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                return response.Models ?? new List<Feedback>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get feedback for user {UserId}", userId);
                throw new AppException($"Failed to get user feedback: {ex.Message}", ex);
            }
        }

        public async Task<Feedback> CreateFeedbackAsync(FeedbackDto feedbackDto, Guid userId)
        {
            ValidationHelper.ValidateModel(feedbackDto);

            var feedback = new Feedback
            {
                Id = Guid.NewGuid(),
                UserId = userId, // From JWT token, not from client
                Message = feedbackDto.Message,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var response = await _supabase.Client.From<Feedback>().Insert(feedback);
                var createdFeedback = response.Models?.FirstOrDefault();

                if (createdFeedback == null)
                    throw new AppException("Failed to create feedback - no data returned");

                _logger.LogInformation("Feedback created successfully with ID {FeedbackId} for user {UserId}", createdFeedback.Id, userId);
                return createdFeedback;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create feedback for user {UserId}", userId);
                throw;
            }
        }

        public async Task DeleteFeedbackAsync(Guid id, Guid userId)
        {
            var feedback = await GetFeedbackByIdForUser(id, userId);

            try
            {
                await _supabase.Client.From<Feedback>().Delete(feedback);
                _logger.LogInformation("Feedback {FeedbackId} deleted successfully by user {UserId}", id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete feedback {FeedbackId} for user {UserId}", id, userId);
                throw new AppException($"Failed to delete feedback: {ex.Message}", ex);
            }
        }

        private async Task<Feedback> GetFeedbackByIdForUser(Guid id, Guid userId)
        {
            var response = await _supabase.Client.From<Feedback>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();

            var feedback = response.Models?.FirstOrDefault();
            if (feedback == null)
                throw new NotFoundException($"Feedback with ID {id} not found");

            // Authorization check: Only allow the owner to delete
            if (feedback.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own feedback");

            return feedback;
        }
    }
}
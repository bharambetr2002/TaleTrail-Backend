using TaleTrail.API.Models;
using TaleTrail.API.DTOs;

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

        public async Task<List<Feedback>> GetAllAsync()
        {
            var response = await _supabase.Client.From<Feedback>().Get();
            return response.Models ?? new List<Feedback>();
        }

        public async Task<List<Feedback>> GetByUserIdAsync(Guid userId)
        {
            var response = await _supabase.Client.From<Feedback>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();
            return response.Models ?? new List<Feedback>();
        }

        public async Task<Feedback?> GetByIdAsync(Guid id)
        {
            var response = await _supabase.Client.From<Feedback>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();
            return response.Models?.FirstOrDefault();
        }

        public async Task<Feedback?> CreateAsync(FeedbackDto dto, Guid userId)
        {
            var feedback = new Feedback
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow
            };
            var response = await _supabase.Client.From<Feedback>().Insert(feedback);
            return response.Models?.FirstOrDefault();
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var response = await _supabase.Client.From<Feedback>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Get();
            var feedback = response.Models?.FirstOrDefault();
            if (feedback == null || feedback.UserId != userId) return false;
            await _supabase.Client.From<Feedback>().Delete(feedback);
            return true;
        }
    }
}
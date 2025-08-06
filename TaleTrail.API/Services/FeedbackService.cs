using TaleTrail.API.DTOs;
using TaleTrail.API.Models;
using TaleTrail.API.Data;
using Microsoft.EntityFrameworkCore;

namespace TaleTrail.API.Services
{
    public class FeedbackService
    {
        private readonly TaleTrailDbContext _context;

        public FeedbackService(TaleTrailDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks.ToListAsync();
        }

        public async Task<Feedback?> GetByIdAsync(Guid id)
        {
            return await _context.Feedbacks.FindAsync(id);
        }

        public async Task<Feedback> CreateAsync(FeedbackDto dto)
        {
            var feedback = new Feedback
            {
                Id = Guid.NewGuid(),
                Message = dto.Message,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return feedback;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return false;

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
using TaleTrail.API.DTOs;
using TaleTrail.API.Models;
using TaleTrail.API.Data;
using Microsoft.EntityFrameworkCore;

namespace TaleTrail.API.Services
{
    public class SubscriptionService
    {
        private readonly TaleTrailDbContext _context;

        public SubscriptionService(TaleTrailDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subscription>> GetAllAsync()
        {
            return await _context.Subscriptions.ToListAsync();
        }

        public async Task<Subscription?> GetByIdAsync(Guid id)
        {
            return await _context.Subscriptions.FindAsync(id);
        }

        public async Task<Subscription> CreateAsync(SubscriptionDto dto)
        {
            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Plan = dto.Plan,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<bool> CancelAsync(Guid id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null) return false;

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
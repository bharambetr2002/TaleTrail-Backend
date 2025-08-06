using TaleTrail.API.DTOs;
using TaleTrail.API.Models;
using TaleTrail.API.Data;
using Microsoft.EntityFrameworkCore;

namespace TaleTrail.API.Services
{
    public class UserService
    {
        private readonly TaleTrailDbContext _context;

        public UserService(TaleTrailDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateAsync(UserDto dto)
        {
            var user = new User
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Email = dto.Email,
                Bio = dto.Bio,
                AvatarUrl = dto.AvatarUrl,
                IsAdmin = dto.IsAdmin,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
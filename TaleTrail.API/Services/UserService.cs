using TaleTrail.API.DAO;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.DTOs.Profile;
using TaleTrail.API.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.Services
{
    public class UserService
    {
        private readonly UserDao _userDao;
        private readonly UserBookDao _userBookDao;
        private readonly ReviewDao _reviewDao;
        private readonly BlogDao _blogDao;
        private readonly ILogger<UserService> _logger;

        public UserService(UserDao userDao, UserBookDao userBookDao, ReviewDao reviewDao, BlogDao blogDao, ILogger<UserService> logger)
        {
            _userDao = userDao;
            _userBookDao = userBookDao;
            _reviewDao = reviewDao;
            _blogDao = blogDao;
            _logger = logger;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _userDao.GetByIdAsync(id);
        }

        public async Task<User?> UpdateUserAsync(Guid userId, UserDto userDto)
        {
            var existingUser = await _userDao.GetByIdAsync(userId);
            if (existingUser == null)
            {
                return null; // Not found
            }

            existingUser.FullName = userDto.FullName;
            existingUser.Bio = userDto.Bio;
            existingUser.AvatarUrl = userDto.AvatarUrl;
            existingUser.Location = userDto.Location;

            return await _userDao.UpdateAsync(existingUser);
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var existingUser = await _userDao.GetByIdAsync(userId);
            if (existingUser == null)
            {
                return false; // Not found
            }

            // Important: You would also need logic here to delete the user from Supabase Auth.
            // This requires using the Supabase Admin client, which is a more advanced setup.
            await _userDao.DeleteAsync(userId);
            return true;
        }

        public async Task<PublicProfileDto?> GetPublicProfileByUsernameAsync(string username)
        {
            var user = await _userDao.GetByUsernameAsync(username);
            if (user == null)
            {
                return null; // User not found
            }

            // Fetch all necessary data in parallel
            var userBooksTask = _userBookDao.GetByUserIdAsync(user.Id);
            var userReviewsTask = _reviewDao.GetByUserIdAsync(user.Id);
            var userBlogsTask = _blogDao.GetAllAsync(user.Id);

            await Task.WhenAll(userBooksTask, userReviewsTask, userBlogsTask);

            var userBooks = await userBooksTask;
            var userReviews = await userReviewsTask;
            var userBlogs = await userBlogsTask;

            // Calculate stats
            var stats = new UserStatsDto
            {
                BooksCompleted = userBooks.Count(ub => ub.Status == "completed"),
                CurrentlyReading = userBooks.Count(ub => ub.Status == "in_progress"),
                WishlistCount = userBooks.Count(ub => ub.Status == "wanna_read"),
                AverageRating = userReviews.Any() ? Math.Round(userReviews.Average(r => r.Rating), 2) : 0,
                BlogsWritten = userBlogs.Count
            };

            // Shape the response DTO
            var profileDto = new PublicProfileDto
            {
                Username = user.Username,
                FullName = user.FullName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                Location = user.Location,
                JoinedAt = user.CreatedAt,
                Stats = stats,
                // For simplicity, we're not including the book list here, but you could add it.
            };

            return profileDto;
        }
    }
}

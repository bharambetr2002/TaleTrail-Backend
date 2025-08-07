using TaleTrail.API.DAO;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.DTOs.Profile;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TaleTrail.API.Services
{
    public class UserService
    {
        private readonly UserDao _userDao;
        private readonly UserBookDao _userBookDao;
        private readonly ReviewDao _reviewDao;
        private readonly BlogDao _blogDao;

        public UserService(UserDao userDao, UserBookDao userBookDao, ReviewDao reviewDao, BlogDao blogDao)
        {
            _userDao = userDao;
            _userBookDao = userBookDao;
            _reviewDao = reviewDao;
            _blogDao = blogDao;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _userDao.GetByIdAsync(id);
        }

        public async Task<User?> UpdateUserAsync(Guid userId, UserDto userDto)
        {
            var existingUser = await _userDao.GetByIdAsync(userId);
            if (existingUser == null) return null;

            existingUser.FullName = userDto.FullName;
            existingUser.Bio = userDto.Bio;
            existingUser.AvatarUrl = userDto.AvatarUrl;
            existingUser.Location = userDto.Location;

            return await _userDao.UpdateAsync(existingUser);
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var existingUser = await _userDao.GetByIdAsync(userId);
            if (existingUser == null) return false;

            await _userDao.DeleteAsync(userId);
            return true;
        }

        public async Task<PublicProfileDto?> GetPublicProfileByUsernameAsync(string username)
        {
            var user = await _userDao.GetByUsernameAsync(username);
            if (user == null) return null;

            var userBooks = await _userBookDao.GetByUserIdAsync(user.Id);
            var userReviews = await _reviewDao.GetByUserIdAsync(user.Id);
            var userBlogs = await _blogDao.GetAllAsync(user.Id);

            var stats = new UserStatsDto
            {
                BooksCompleted = userBooks.Count(ub => ub.Status == "completed"),
                CurrentlyReading = userBooks.Count(ub => ub.Status == "in_progress"),
                WishlistCount = userBooks.Count(ub => ub.Status == "wanna_read"),
                AverageRating = userReviews.Any() ? Math.Round(userReviews.Average(r => r.Rating), 2) : 0,
                BlogsWritten = userBlogs.Count
            };

            return new PublicProfileDto
            {
                Username = user.Username,
                FullName = user.FullName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                Location = user.Location,
                JoinedAt = user.CreatedAt,
                Stats = stats,
            };
        }

        /// <summary>
        /// A special method for creating a user directly from the BaseController's self-healing mechanism.
        /// </summary>
        public async Task<User?> CreateUserFromJwtAsync(User user)
        {
            // Simply pass the already-constructed user object to the DAO to be saved.
            return await _userDao.AddAsync(user);
        }
    }
}
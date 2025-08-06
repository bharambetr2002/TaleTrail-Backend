using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Services
{
    public class UserService
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<UserService> _logger;

        public UserService(SupabaseService supabase, ILogger<UserService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var response = await _supabase.Client.From<User>()
                    .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                return response.Models ?? new List<User>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all users");
                throw new AppException($"Failed to get users: {ex.Message}", ex);
            }
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            try
            {
                var response = await _supabase.Client.From<User>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                return response.Models?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user {UserId}", id);
                throw new AppException($"Failed to get user: {ex.Message}", ex);
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                var response = await _supabase.Client.From<User>()
                    .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
                    .Get();

                return response.Models?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user by email {Email}", email);
                throw new AppException($"Failed to get user by email: {ex.Message}", ex);
            }
        }

        public async Task<User> UpdateUserProfileAsync(Guid userId, UserDto userDto)
        {
            ValidationHelper.ValidateModel(userDto);

            var existingUser = await GetUserByIdAsync(userId);
            if (existingUser == null)
                throw new NotFoundException($"User with ID {userId} not found");

            existingUser.FullName = userDto.FullName;
            existingUser.Bio = userDto.Bio;
            existingUser.AvatarUrl = userDto.AvatarUrl;

            try
            {
                var response = await _supabase.Client.From<User>().Update(existingUser);
                var updatedUser = response.Models?.FirstOrDefault();

                if (updatedUser == null)
                    throw new AppException("Failed to update user profile");

                _logger.LogInformation("User profile {UserId} updated successfully", userId);
                return updatedUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user profile {UserId}", userId);
                throw;
            }
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with ID {userId} not found");

            try
            {
                await _supabase.Client.From<User>().Delete(user);
                _logger.LogInformation("User {UserId} deleted successfully", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user {UserId}", userId);
                throw new AppException($"Failed to delete user: {ex.Message}", ex);
            }
        }

        // Admin-only methods
        public async Task<List<User>> SearchUsersAsync(string searchTerm)
        {
            try
            {
                var response = await _supabase.Client.From<User>()
                    .Filter("full_name", Supabase.Postgrest.Constants.Operator.ILike, $"%{searchTerm}%")
                    .Or(new List<Supabase.Postgrest.Interfaces.IPostgrestQueryFilter>
                    {
                        new Supabase.Postgrest.QueryFilter("email", Supabase.Postgrest.Constants.Operator.ILike, $"%{searchTerm}%")
                    })
                    .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                return response.Models ?? new List<User>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search users with term: {SearchTerm}", searchTerm);
                throw new AppException($"Failed to search users: {ex.Message}", ex);
            }
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            try
            {
                var response = await _supabase.Client.From<User>().Get();
                return response.Models?.Count ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get total users count");
                throw new AppException($"Failed to get users count: {ex.Message}", ex);
            }
        }
    }
}
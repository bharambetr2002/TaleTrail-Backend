using TaleTrail.API.DAO;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TaleTrail.API.Services
{
    public class UserBookService
    {
        private readonly UserBookDao _userBookDao;
        private readonly ILogger<UserBookService> _logger;

        public UserBookService(UserBookDao userBookDao, ILogger<UserBookService> logger)
        {
            _userBookDao = userBookDao;
            _logger = logger;
        }

        public async Task<List<UserBook>> GetUserReadingListAsync(Guid userId)
        {
            return await _userBookDao.GetByUserIdAsync(userId);
        }

        public async Task<UserBook> AddOrUpdateUserBookAsync(Guid userId, UserBookDTO userBookDto)
        {
            // Business Rule: Check if user is trying to add a 4th "in_progress" book
            if (userBookDto.Status.ToLower() == "in_progress")
            {
                var inProgressCount = await _userBookDao.GetInProgressCountAsync(userId);
                var existingEntry = await _userBookDao.GetByUserAndBookAsync(userId, userBookDto.BookId);

                // Only throw an error if this is a NEW book being moved to in_progress,
                // not an existing in_progress book being updated.
                if (inProgressCount >= 3 && (existingEntry == null || existingEntry.Status != "in_progress"))
                {
                    throw new AppException("You cannot have more than 3 books in progress at the same time.");
                }
            }

            // Check if an entry already exists for this user and book
            var existingUserBook = await _userBookDao.GetByUserAndBookAsync(userId, userBookDto.BookId);

            if (existingUserBook != null)
            {
                // Update existing entry
                existingUserBook.Status = userBookDto.Status;
                // Set started_at or completed_at based on status
                if (userBookDto.Status.ToLower() == "in_progress" && existingUserBook.StartedAt == null)
                {
                    existingUserBook.StartedAt = DateTime.UtcNow;
                }
                else if (userBookDto.Status.ToLower() == "completed")
                {
                    existingUserBook.CompletedAt = DateTime.UtcNow;
                }

                var updatedUserBook = await _userBookDao.UpdateAsync(existingUserBook);
                if (updatedUserBook == null) throw new AppException("Failed to update book status.");
                return updatedUserBook;
            }
            else
            {
                // Create new entry
                var newUserBook = new UserBook
                {
                    UserId = userId,
                    BookId = userBookDto.BookId,
                    Status = userBookDto.Status,
                    AddedAt = DateTime.UtcNow
                };

                if (userBookDto.Status.ToLower() == "in_progress")
                {
                    newUserBook.StartedAt = DateTime.UtcNow;
                }

                var addedUserBook = await _userBookDao.AddAsync(newUserBook);
                if (addedUserBook == null) throw new AppException("Failed to add book to your list.");
                return addedUserBook;
            }
        }

        public async Task<bool> RemoveUserBookAsync(Guid userId, Guid bookId)
        {
            var existingUserBook = await _userBookDao.GetByUserAndBookAsync(userId, bookId);
            if (existingUserBook == null)
            {
                return false; // Not found
            }

            await _userBookDao.DeleteAsync(userId, bookId);
            return true;
        }
    }
}
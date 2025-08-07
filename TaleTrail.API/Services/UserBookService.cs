using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.DAO;

namespace TaleTrail.API.Services;

public class UserBookService
{
    private readonly UserBookDao _userBookDao;
    private readonly BookDao _bookDao;

    public UserBookService(UserBookDao userBookDao, BookDao bookDao)
    {
        _userBookDao = userBookDao;
        _bookDao = bookDao;
    }

    public async Task<List<UserBook>> GetUserBooksAsync(Guid userId)
    {
        return await _userBookDao.GetByUserIdAsync(userId);
    }

    public async Task<List<UserBook>> GetInProgressBooksAsync(Guid userId)
    {
        return await _userBookDao.GetInProgressByUserAsync(userId);
    }

    public async Task<UserBook> AddBookToUserListAsync(Guid userId, AddUserBookRequest request)
    {
        // Check if book exists
        var book = await _bookDao.GetByIdAsync(request.BookId);
        if (book == null)
            throw new Exception("Book not found");

        // Check if user already has this book
        var existingUserBook = await _userBookDao.GetByUserAndBookAsync(userId, request.BookId);
        if (existingUserBook != null)
            throw new Exception("Book already in user's list");

        // Business rule: Maximum 3 books "in progress"
        if (request.ReadingStatus == ReadingStatus.InProgress)
        {
            var inProgressBooks = await _userBookDao.GetInProgressByUserAsync(userId);
            if (inProgressBooks.Count >= 3)
                throw new Exception("You can have a maximum of 3 books in progress");
        }

        var userBook = new UserBook
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookId = request.BookId,
            ReadingStatus = request.ReadingStatus,
            Progress = request.Progress,
            StartedAt = request.ReadingStatus == ReadingStatus.InProgress ? DateTime.UtcNow : null,
            CompletedAt = request.ReadingStatus == ReadingStatus.Completed ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _userBookDao.CreateAsync(userBook);
    }

    public async Task<UserBook> UpdateUserBookAsync(Guid userId, Guid bookId, UpdateUserBookRequest request)
    {
        var existingUserBook = await _userBookDao.GetByUserAndBookAsync(userId, bookId);
        if (existingUserBook == null)
            throw new Exception("Book not found in user's list");

        // Business rule: Maximum 3 books "in progress"
        if (request.ReadingStatus == ReadingStatus.InProgress &&
            existingUserBook.ReadingStatus != ReadingStatus.InProgress)
        {
            var inProgressBooks = await _userBookDao.GetInProgressByUserAsync(userId);
            if (inProgressBooks.Count >= 3)
                throw new Exception("You can have a maximum of 3 books in progress");
        }

        existingUserBook.ReadingStatus = request.ReadingStatus;
        existingUserBook.Progress = request.Progress;

        // Update timestamps based on status
        if (request.ReadingStatus == ReadingStatus.InProgress && existingUserBook.StartedAt == null)
            existingUserBook.StartedAt = DateTime.UtcNow;

        if (request.ReadingStatus == ReadingStatus.Completed)
            existingUserBook.CompletedAt = DateTime.UtcNow;
        else if (existingUserBook.ReadingStatus == ReadingStatus.Completed)
            existingUserBook.CompletedAt = null; // Reset if moving away from completed

        return await _userBookDao.UpdateAsync(existingUserBook);
    }

    public async Task RemoveBookFromUserListAsync(Guid userId, Guid bookId)
    {
        var existingUserBook = await _userBookDao.GetByUserAndBookAsync(userId, bookId);
        if (existingUserBook == null)
            throw new Exception("Book not found in user's list");

        await _userBookDao.DeleteAsync(userId, bookId);
    }
}
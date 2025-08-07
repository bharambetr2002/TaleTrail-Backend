using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.DAO;

namespace TaleTrail.API.Services;

public class UserBookService
{
    private readonly UserBookDao _userBookDao;
    private readonly BookDao _bookDao;
    private readonly ILogger<UserBookService> _logger;

    public UserBookService(UserBookDao userBookDao, BookDao bookDao, ILogger<UserBookService> logger)
    {
        _userBookDao = userBookDao;
        _bookDao = bookDao;
        _logger = logger;
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
        // CRITICAL FIX: Validate book exists
        var book = await _bookDao.GetByIdAsync(request.BookId);
        if (book == null)
            throw new KeyNotFoundException($"Book with ID {request.BookId} not found");

        // Check if user already has this book
        var existingUserBook = await _userBookDao.GetByUserAndBookAsync(userId, request.BookId);
        if (existingUserBook != null)
            throw new InvalidOperationException("Book already in user's list");

        // Business rule: Maximum 3 books "in progress"
        if (request.ReadingStatus == ReadingStatus.InProgress)
        {
            var inProgressBooks = await _userBookDao.GetInProgressByUserAsync(userId);
            if (inProgressBooks.Count >= 3)
                throw new InvalidOperationException("You can have a maximum of 3 books in progress");
        }

        // FIXED: Consistent timestamp logic
        var now = DateTime.UtcNow;
        DateTime? startedAt = null;
        DateTime? completedAt = null;

        switch (request.ReadingStatus)
        {
            case ReadingStatus.InProgress:
                startedAt = now;
                break;
            case ReadingStatus.Completed:
                // If completed directly, assume it was started and finished now
                startedAt = now;
                completedAt = now;
                request.Progress = 100; // Force progress to 100 for completed books
                break;
            case ReadingStatus.ToRead:
            case ReadingStatus.Dropped:
                // No timestamps needed
                break;
        }

        var userBook = new UserBook
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookId = request.BookId,
            ReadingStatus = request.ReadingStatus,
            Progress = Math.Max(0, Math.Min(100, request.Progress)), // Ensure progress is 0-100
            StartedAt = startedAt,
            CompletedAt = completedAt,
            CreatedAt = now,
            UpdatedAt = now
        };

        try
        {
            return await _userBookDao.CreateAsync(userBook);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add book {BookId} to user {UserId} list", request.BookId, userId);
            throw new InvalidOperationException("Failed to add book to list", ex);
        }
    }

    public async Task<UserBook> UpdateUserBookAsync(Guid userId, Guid bookId, UpdateUserBookRequest request)
    {
        var existingUserBook = await _userBookDao.GetByUserAndBookAsync(userId, bookId);
        if (existingUserBook == null)
            throw new KeyNotFoundException("Book not found in user's list");

        // Business rule: Maximum 3 books "in progress"
        if (request.ReadingStatus == ReadingStatus.InProgress &&
            existingUserBook.ReadingStatus != ReadingStatus.InProgress)
        {
            var inProgressBooks = await _userBookDao.GetInProgressByUserAsync(userId);
            if (inProgressBooks.Count >= 3)
                throw new InvalidOperationException("You can have a maximum of 3 books in progress");
        }

        var oldStatus = existingUserBook.ReadingStatus;
        var now = DateTime.UtcNow;

        // Update basic fields
        existingUserBook.ReadingStatus = request.ReadingStatus;
        existingUserBook.Progress = Math.Max(0, Math.Min(100, request.Progress));

        // FIXED: Consistent timestamp and progress logic
        switch (request.ReadingStatus)
        {
            case ReadingStatus.InProgress:
                if (oldStatus != ReadingStatus.InProgress && existingUserBook.StartedAt == null)
                {
                    existingUserBook.StartedAt = now;
                }
                // Clear completed timestamp if moving back to in-progress
                if (oldStatus == ReadingStatus.Completed)
                {
                    existingUserBook.CompletedAt = null;
                }
                break;

            case ReadingStatus.Completed:
                // Set timestamps appropriately
                if (existingUserBook.StartedAt == null)
                {
                    existingUserBook.StartedAt = now; // Assume started now if not set
                }
                existingUserBook.CompletedAt = now;
                existingUserBook.Progress = 100; // Force progress to 100
                break;

            case ReadingStatus.ToRead:
                // Reset all progress when moving back to "to read"
                existingUserBook.Progress = 0;
                existingUserBook.StartedAt = null;
                existingUserBook.CompletedAt = null;
                break;

            case ReadingStatus.Dropped:
                // Keep started date but clear completed date
                existingUserBook.CompletedAt = null;
                // Don't reset progress - user might want to resume later
                break;
        }

        try
        {
            return await _userBookDao.UpdateAsync(existingUserBook);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update book {BookId} for user {UserId}", bookId, userId);
            throw new InvalidOperationException("Failed to update book status", ex);
        }
    }

    public async Task RemoveBookFromUserListAsync(Guid userId, Guid bookId)
    {
        var existingUserBook = await _userBookDao.GetByUserAndBookAsync(userId, bookId);
        if (existingUserBook == null)
            throw new KeyNotFoundException("Book not found in user's list");

        try
        {
            await _userBookDao.DeleteAsync(userId, bookId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove book {BookId} from user {UserId} list", bookId, userId);
            throw new InvalidOperationException("Failed to remove book from list", ex);
        }
    }
}
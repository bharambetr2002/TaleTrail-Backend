using TaleTrail.API.DAO;
using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;

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

    public async Task<List<UserBookResponseDTO>> GetUserBooksAsync(Guid userId)
    {
        var userBooks = await _userBookDao.GetByUserIdAsync(userId);
        var userBookDtos = new List<UserBookResponseDTO>();

        foreach (var userBook in userBooks)
        {
            var book = await _bookDao.GetByIdAsync(userBook.BookId);

            userBookDtos.Add(new UserBookResponseDTO
            {
                Id = userBook.Id,
                BookId = userBook.BookId,
                BookTitle = book?.Title ?? "Unknown Book",
                BookCoverUrl = book?.CoverImageUrl,
                ReadingStatus = userBook.ReadingStatus,
                Progress = userBook.Progress,
                StartedAt = userBook.StartedAt,
                CompletedAt = userBook.CompletedAt,
                AddedAt = userBook.CreatedAt
            });
        }

        return userBookDtos;
    }

    public async Task<UserBookResponseDTO> AddBookToUserListAsync(Guid userId, AddUserBookRequestDTO request)
    {
        // Validate book exists
        var book = await _bookDao.GetByIdAsync(request.BookId);
        if (book == null)
            throw new KeyNotFoundException("Book not found");

        // Check if already in list
        var existing = await _userBookDao.GetByUserAndBookAsync(userId, request.BookId);
        if (existing != null)
            throw new InvalidOperationException("Book already in your list");

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

        if (request.ReadingStatus == ReadingStatus.Completed)
            userBook.Progress = 100;

        var createdUserBook = await _userBookDao.CreateAsync(userBook);

        return new UserBookResponseDTO
        {
            Id = createdUserBook.Id,
            BookId = createdUserBook.BookId,
            BookTitle = book.Title,
            BookCoverUrl = book.CoverImageUrl,
            ReadingStatus = createdUserBook.ReadingStatus,
            Progress = createdUserBook.Progress,
            StartedAt = createdUserBook.StartedAt,
            CompletedAt = createdUserBook.CompletedAt,
            AddedAt = createdUserBook.CreatedAt
        };
    }

    public async Task<UserBookResponseDTO> UpdateUserBookAsync(Guid userId, Guid bookId, UpdateUserBookRequestDTO request)
    {
        var userBook = await _userBookDao.GetByUserAndBookAsync(userId, bookId);
        if (userBook == null)
            throw new KeyNotFoundException("Book not found in your list");

        var book = await _bookDao.GetByIdAsync(bookId);

        userBook.ReadingStatus = request.ReadingStatus;
        userBook.Progress = request.Progress;
        userBook.UpdatedAt = DateTime.UtcNow;

        // Update timestamps based on status
        switch (request.ReadingStatus)
        {
            case ReadingStatus.InProgress:
                if (userBook.StartedAt == null)
                    userBook.StartedAt = DateTime.UtcNow;
                userBook.CompletedAt = null;
                break;
            case ReadingStatus.Completed:
                if (userBook.StartedAt == null)
                    userBook.StartedAt = DateTime.UtcNow;
                userBook.CompletedAt = DateTime.UtcNow;
                userBook.Progress = 100;
                break;
            case ReadingStatus.ToRead:
                userBook.StartedAt = null;
                userBook.CompletedAt = null;
                userBook.Progress = 0;
                break;
            case ReadingStatus.Dropped:
                userBook.CompletedAt = null;
                break;
        }

        var updatedUserBook = await _userBookDao.UpdateAsync(userBook);

        return new UserBookResponseDTO
        {
            Id = updatedUserBook.Id,
            BookId = updatedUserBook.BookId,
            BookTitle = book?.Title ?? "Unknown Book",
            BookCoverUrl = book?.CoverImageUrl,
            ReadingStatus = updatedUserBook.ReadingStatus,
            Progress = updatedUserBook.Progress,
            StartedAt = updatedUserBook.StartedAt,
            CompletedAt = updatedUserBook.CompletedAt,
            AddedAt = updatedUserBook.CreatedAt
        };
    }

    public async Task RemoveBookFromUserListAsync(Guid userId, Guid bookId)
    {
        var userBook = await _userBookDao.GetByUserAndBookAsync(userId, bookId);
        if (userBook == null)
            throw new KeyNotFoundException("Book not found in your list");

        await _userBookDao.DeleteAsync(userId, bookId);
    }
}
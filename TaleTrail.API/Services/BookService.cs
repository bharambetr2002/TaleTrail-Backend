using TaleTrail.API.DAO;
using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;

namespace TaleTrail.API.Services;

public class BookService
{
    private readonly BookDao _bookDao;
    private readonly AuthorDao _authorDao;
    private readonly PublisherDao _publisherDao;

    public BookService(BookDao bookDao, AuthorDao authorDao, PublisherDao publisherDao)
    {
        _bookDao = bookDao;
        _authorDao = authorDao;
        _publisherDao = publisherDao;
    }

    public async Task<List<BookResponseDTO>> GetAllBooksAsync(string? search = null)
    {
        var books = string.IsNullOrWhiteSpace(search)
            ? await _bookDao.GetAllAsync()
            : await _bookDao.SearchByTitleAsync(search);

        var bookDtos = new List<BookResponseDTO>();

        foreach (var book in books)
        {
            var bookDto = await MapToBookResponseDTO(book);
            bookDtos.Add(bookDto);
        }

        return bookDtos;
    }

    public async Task<BookResponseDTO?> GetBookByIdAsync(Guid id)
    {
        var book = await _bookDao.GetByIdAsync(id);
        if (book == null) return null;

        return await MapToBookResponseDTO(book);
    }

    public async Task<List<BookResponseDTO>> GetBooksByAuthorAsync(Guid authorId)
    {
        var books = await _bookDao.GetByAuthorIdAsync(authorId);
        var bookDtos = new List<BookResponseDTO>();

        foreach (var book in books)
        {
            var bookDto = await MapToBookResponseDTO(book);
            bookDtos.Add(bookDto);
        }

        return bookDtos;
    }

    private async Task<BookResponseDTO> MapToBookResponseDTO(Book book)
    {
        var bookDto = new BookResponseDTO
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Language = book.Language,
            CoverImageUrl = book.CoverImageUrl,
            PublicationYear = book.PublicationYear,
            PublisherId = book.PublisherId
        };

        // Get publisher name if exists
        if (book.PublisherId.HasValue)
        {
            var publisher = await _publisherDao.GetByIdAsync(book.PublisherId.Value);
            bookDto.PublisherName = publisher?.Name;
        }

        // Get authors
        var authors = await _authorDao.GetByBookIdAsync(book.Id);
        bookDto.Authors = authors.Select(a => new AuthorResponseDTO
        {
            Id = a.Id,
            Name = a.Name,
            Bio = a.Bio,
            BirthDate = a.BirthDate,
            DeathDate = a.DeathDate
        }).ToList();

        return bookDto;
    }
}
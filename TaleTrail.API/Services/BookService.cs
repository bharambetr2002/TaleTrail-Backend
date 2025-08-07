using TaleTrail.API.DAO;
using TaleTrail.API.Model;

namespace TaleTrail.API.Services;

public class BookService
{
    private readonly BookDao _bookDao;

    public BookService(BookDao bookDao)
    {
        _bookDao = bookDao;
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _bookDao.GetAllAsync();
    }

    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        return await _bookDao.GetByIdAsync(id);
    }

    public async Task<List<Book>> SearchBooksAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllBooksAsync();

        return await _bookDao.SearchByTitleAsync(searchTerm);
    }
}
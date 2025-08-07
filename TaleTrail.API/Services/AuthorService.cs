using TaleTrail.API.Model;
using TaleTrail.API.DAO;

namespace TaleTrail.API.Services;

public class AuthorService
{
    private readonly AuthorDao _authorDao;

    public AuthorService(AuthorDao authorDao)
    {
        _authorDao = authorDao;
    }

    public async Task<List<Author>> GetAllAuthorsAsync()
    {
        return await _authorDao.GetAllAsync();
    }

    public async Task<Author?> GetAuthorByIdAsync(Guid id)
    {
        return await _authorDao.GetByIdAsync(id);
    }
}
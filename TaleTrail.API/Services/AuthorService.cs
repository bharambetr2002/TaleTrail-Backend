using TaleTrail.API.Model;
using TaleTrail.API.Model.DTOs;
using TaleTrail.API.DAO;

namespace TaleTrail.API.Services;

public class AuthorService
{
    private readonly AuthorDao _authorDao;

    public AuthorService(AuthorDao authorDao)
    {
        _authorDao = authorDao;
    }

    public async Task<List<AuthorResponseDTO>> GetAllAuthorsAsync()
    {
        var authors = await _authorDao.GetAllAsync();
        return authors.Select(MapToAuthorResponseDTO).ToList();
    }

    public async Task<AuthorResponseDTO?> GetAuthorByIdAsync(Guid id)
    {
        var author = await _authorDao.GetByIdAsync(id);
        return author == null ? null : MapToAuthorResponseDTO(author);
    }

    private static AuthorResponseDTO MapToAuthorResponseDTO(Author author)
    {
        return new AuthorResponseDTO
        {
            Id = author.Id,
            Name = author.Name,
            Bio = author.Bio,
            BirthDate = author.BirthDate,
            DeathDate = author.DeathDate,
            BookCount = 0 // TODO: Calculate book count if needed
        };
    }
}
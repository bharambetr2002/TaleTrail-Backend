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
    public class AuthorService
    {
        private readonly AuthorDao _authorDao;
        private readonly ILogger<AuthorService> _logger;

        public AuthorService(AuthorDao authorDao, ILogger<AuthorService> logger)
        {
            _authorDao = authorDao;
            _logger = logger;
        }

        public async Task<List<Author>> GetAllAuthorsAsync(string? searchTerm = null)
        {
            return await _authorDao.GetAllAsync(searchTerm);
        }

        public async Task<Author?> GetAuthorByIdAsync(Guid id)
        {
            return await _authorDao.GetByIdAsync(id);
        }

        public async Task<Author> AddAuthorAsync(AuthorDto authorDto)
        {
            var author = new Author
            {
                Name = authorDto.Name
            };

            var createdAuthor = await _authorDao.AddAsync(author);
            if (createdAuthor == null)
            {
                throw new AppException("Failed to create author.");
            }
            return createdAuthor;
        }

        public async Task<Author?> UpdateAuthorAsync(Guid id, AuthorDto authorDto)
        {
            var existingAuthor = await _authorDao.GetByIdAsync(id);
            if (existingAuthor == null)
            {
                return null; // Not found
            }

            existingAuthor.Name = authorDto.Name;
            return await _authorDao.UpdateAsync(existingAuthor);
        }

        public async Task<bool> DeleteAuthorAsync(Guid id)
        {
            var existingAuthor = await _authorDao.GetByIdAsync(id);
            if (existingAuthor == null)
            {
                return false; // Not found
            }

            await _authorDao.DeleteAsync(id);
            return true;
        }
    }
}

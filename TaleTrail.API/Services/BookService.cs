using TaleTrail.API.DAO;
using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaleTrail.API.Services
{
    public class BookService
    {
        private readonly BookDao _bookDao;
        private readonly BookAuthorDao _bookAuthorDao;

        public BookService(BookDao bookDao, BookAuthorDao bookAuthorDao)
        {
            _bookDao = bookDao;
            _bookAuthorDao = bookAuthorDao;
        }

        public async Task<List<Book>> GetAllBooksAsync(string? searchTerm = null)
        {
            return await _bookDao.GetAllAsync(searchTerm);
        }

        public async Task<Book?> GetBookByIdAsync(Guid id)
        {
            return await _bookDao.GetByIdAsync(id);
        }

        public async Task<Book> CreateBookAsync(BookDto bookDto)
        {
            var book = new Book
            {
                Title = bookDto.Title,
                Description = bookDto.Description,
                CoverUrl = bookDto.CoverUrl,
                PublicationYear = bookDto.PublicationYear,
                PublisherId = bookDto.PublisherId,
                CreatedAt = DateTime.UtcNow
            };

            var createdBook = await _bookDao.AddAsync(book);
            if (createdBook == null)
            {
                throw new AppException("Failed to create book.");
            }

            foreach (var authorId in bookDto.AuthorIds)
            {
                var bookAuthorLink = new BookAuthor { BookId = createdBook.Id, AuthorId = authorId };
                await _bookAuthorDao.AddAsync(bookAuthorLink);
            }

            return createdBook;
        }

        // --- METHOD RESTORED ---
        public async Task<Book?> UpdateBookAsync(Guid id, BookDto bookDto)
        {
            var existingBook = await _bookDao.GetByIdAsync(id);
            if (existingBook == null)
            {
                return null; // Not found
            }

            existingBook.Title = bookDto.Title;
            existingBook.Description = bookDto.Description;
            existingBook.CoverUrl = bookDto.CoverUrl;
            existingBook.PublicationYear = bookDto.PublicationYear;
            existingBook.PublisherId = bookDto.PublisherId;

            return await _bookDao.UpdateAsync(existingBook);
        }

        // --- METHOD RESTORED ---
        public async Task<bool> DeleteBookAsync(Guid id)
        {
            var existingBook = await _bookDao.GetByIdAsync(id);
            if (existingBook == null)
            {
                return false; // Not found
            }

            await _bookDao.DeleteAsync(id);
            return true;
        }
    }
}
using Microsoft.EntityFrameworkCore;
using TaleTrail.API.Data;
using TaleTrail.API.DTOs;
using TaleTrail.API.Models;
using TaleTrail.API.Services.Interfaces;
using AutoMapper;

namespace TaleTrail.API.Services
{
    public class BookService : IBookService
    {
        private readonly TaleTrailDbContext _context;
        private readonly IMapper _mapper;

        public BookService(TaleTrailDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<BookResponseDTO>> GetAllBooksAsync()
        {
            var books = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Publisher)
                .Include(b => b.Categories)
                .ToListAsync();

            return books.Select(_mapper.Map<BookResponseDTO>).ToList();
        }

        public async Task<BookResponseDTO> GetBookByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Publisher)
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == id);

            return book == null ? null : _mapper.Map<BookResponseDTO>(book);
        }

        public async Task<BookResponseDTO> AddBookAsync(BookRequestDTO dto)
        {
            var book = _mapper.Map<Book>(dto);

            book.Authors = await _context.Authors.Where(a => dto.AuthorIds.Contains(a.Id)).ToListAsync();
            book.Categories = await _context.Categories.Where(c => dto.CategoryIds.Contains(c.Id)).ToListAsync();
            book.Publisher = await _context.Publishers.FindAsync(dto.PublisherId);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return _mapper.Map<BookResponseDTO>(book);
        }

        public async Task<BookResponseDTO> UpdateBookAsync(int id, BookRequestDTO dto)
        {
            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Publisher)
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return null;

            _mapper.Map(dto, book);

            book.Authors = await _context.Authors.Where(a => dto.AuthorIds.Contains(a.Id)).ToListAsync();
            book.Categories = await _context.Categories.Where(c => dto.CategoryIds.Contains(c.Id)).ToListAsync();
            book.Publisher = await _context.Publishers.FindAsync(dto.PublisherId);

            await _context.SaveChangesAsync();

            return _mapper.Map<BookResponseDTO>(book);
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

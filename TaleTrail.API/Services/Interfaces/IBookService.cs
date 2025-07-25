using TaleTrail.API.DTOs;

namespace TaleTrail.API.Services.Interfaces
{
    public interface IBookService
    {
        Task<List<BookResponseDTO>> GetAllBooksAsync();
        Task<BookResponseDTO> GetBookByIdAsync(int id);
        Task<BookResponseDTO> AddBookAsync(BookRequestDTO dto);
        Task<BookResponseDTO> UpdateBookAsync(int id, BookRequestDTO dto);
        Task<bool> DeleteBookAsync(int id);
    }
}

using MvC_SystemMovies.Models;

namespace MvC_SystemMovies.Services.Interfaces
{
    public interface ICinemaService
    {
        Task<IEnumerable<Cinema>> GetAllAsync();
        Task<Cinema?> GetByIdAsync(int id);
        Task CreateAsync(Cinema cinema, IFormFile? image);
        Task<bool> UpdateAsync(Cinema cinema, IFormFile? image);
        Task<bool> DeleteAsync(int id);
    }
}

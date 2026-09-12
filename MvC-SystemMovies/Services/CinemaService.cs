using MvC_SystemMovies.Models;
using MvC_SystemMovies.Repositories.Interfaces;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Services
{
  
    public class CinemaService : ICinemaService
    {
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IFileStorageService _fileStorageService;

        public CinemaService(IRepository<Cinema> cinemaRepository, IFileStorageService fileStorageService)
        {
            _cinemaRepository = cinemaRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<Cinema>> GetAllAsync() => await _cinemaRepository.GetAllAsync();

        public async Task<Cinema?> GetByIdAsync(int id) => await _cinemaRepository.GetByIdAsync(id);

        public async Task CreateAsync(Cinema cinema, IFormFile? image)
        {
            if (image != null)
            {
                cinema.image = await _fileStorageService.SaveImageAsync(image);
            }

            await _cinemaRepository.AddAsync(cinema);
            await _cinemaRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Cinema cinema, IFormFile? image)
        {
            var existing = await _cinemaRepository.GetByIdAsync(cinema.Id);
            if (existing == null)
                return false;

            existing.Name = cinema.Name;

            if (image != null)
            {
                var newFileName = await _fileStorageService.SaveImageAsync(image);
                
                _fileStorageService.DeleteImage(existing.image);
                existing.image = newFileName;
            }

            _cinemaRepository.Update(existing);
            await _cinemaRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cinema = await _cinemaRepository.GetByIdAsync(id);
            if (cinema == null)
                return false;

            _cinemaRepository.Remove(cinema);
            await _cinemaRepository.SaveChangesAsync();

            
            _fileStorageService.DeleteImage(cinema.image);
            return true;
        }
    }
}

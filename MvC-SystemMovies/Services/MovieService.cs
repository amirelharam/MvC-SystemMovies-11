using Microsoft.EntityFrameworkCore;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Repositories.Interfaces;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Services
{
    public class MovieService : IMovieService
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IFileStorageService _fileStorageService;

        public MovieService(IRepository<Movie> movieRepository, IFileStorageService fileStorageService)
        {
            _movieRepository = movieRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<Movie>> GetAllWithDetailsAsync()
        {
            return await _movieRepository.Query()
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.MovieActors!)
                    .ThenInclude(ma => ma.Actor)
                .ToListAsync();
        }

        public async Task<Movie?> GetByIdAsync(int id) => await _movieRepository.GetByIdAsync(id);

        public async Task<MovieListResult> SearchAsync(string? search, string? category, string? cinema, int page, int pageSize = 6)
        {
            if (page < 1) page = 1;

            var query = _movieRepository.Query()
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m => m.Title.Contains(search));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(m => m.Category!.Name == category);

            if (!string.IsNullOrWhiteSpace(cinema))
                query = query.Where(m => m.Cinema!.Name == cinema);

            var totalMovies = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalMovies / (double)pageSize);

            var movies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new MovieListResult
            {
                Movies = movies,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public async Task CreateAsync(Movie movie, IFormFile? image)
        {
            if (image != null)
            {
                movie.Minigm = await _fileStorageService.SaveImageAsync(image);
            }

            await _movieRepository.AddAsync(movie);
            await _movieRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Movie movie, IFormFile? image)
        {
            var existing = await _movieRepository.GetByIdAsync(movie.Id);
            if (existing == null)
                return false;

            existing.Title = movie.Title;
            existing.Description = movie.Description;
            existing.Price = movie.Price;
            existing.Status = movie.Status;
            existing.DateTime = movie.DateTime;
            existing.CategoryId = movie.CategoryId;
            existing.CinemaId = movie.CinemaId;

            if (image != null)
            {
                var newFileName = await _fileStorageService.SaveImageAsync(image);
                _fileStorageService.DeleteImage(existing.Minigm);
                existing.Minigm = newFileName;
            }

            _movieRepository.Update(existing);
            await _movieRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
                return false;

            _movieRepository.Remove(movie);
            await _movieRepository.SaveChangesAsync();

            _fileStorageService.DeleteImage(movie.Minigm);
            return true;
        }
    }
}

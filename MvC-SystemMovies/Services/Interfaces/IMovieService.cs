using MvC_SystemMovies.Models;

namespace MvC_SystemMovies.Services.Interfaces
{
    public class MovieListResult
    {
        public List<Movie> Movies { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetAllWithDetailsAsync();
        Task<Movie?> GetByIdAsync(int id);
        Task<MovieListResult> SearchAsync(string? search, string? category, string? cinema, int page, int pageSize = 6);
        Task CreateAsync(Movie movie, IFormFile? image);
        Task<bool> UpdateAsync(Movie movie, IFormFile? image);
        Task<bool> DeleteAsync(int id);
    }
}

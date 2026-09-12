using MvC_SystemMovies.Models;

namespace MvC_SystemMovies.Services.Interfaces
{
    public interface IReviewService
    {
        Task<List<Review>> GetForMovieAsync(int movieId);
        Task<List<Review>> GetAllAsync();
        Task<double> GetAverageRatingAsync(int movieId);
        Task<bool> HasUserReviewedAsync(string userId, int movieId);
        Task AddAsync(Review review);
        Task<bool> DeleteAsync(int id);
    }
}

using Microsoft.EntityFrameworkCore;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Repositories.Interfaces;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> _reviewRepository;

        public ReviewService(IRepository<Review> reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<Review>> GetForMovieAsync(int movieId)
        {
            return await _reviewRepository.Query()
                .Include(r => r.User)
                .Where(r => r.MovieId == movieId)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<Review>> GetAllAsync()
        {
            return await _reviewRepository.Query()
                .Include(r => r.User)
                .Include(r => r.Movie)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(int movieId)
        {
            var ratings = await _reviewRepository.Query()
                .Where(r => r.MovieId == movieId)
                .Select(r => r.Rating)
                .ToListAsync();

            return ratings.Count == 0 ? 0 : ratings.Average();
        }

        public async Task<bool> HasUserReviewedAsync(string userId, int movieId)
        {
            return await _reviewRepository.Query()
                .AnyAsync(r => r.UserId == userId && r.MovieId == movieId);
        }

        public async Task AddAsync(Review review)
        {
            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null) return false;

            _reviewRepository.Remove(review);
            await _reviewRepository.SaveChangesAsync();
            return true;
        }
    }
}

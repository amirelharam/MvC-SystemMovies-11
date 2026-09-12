using Microsoft.EntityFrameworkCore;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Repositories.Interfaces;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IRepository<Favorite> _favoriteRepository;

        public FavoriteService(IRepository<Favorite> favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<List<Favorite>> GetFavoritesAsync(string userId)
        {
            return await _favoriteRepository.Query()
                .Include(f => f.Movie)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.AddedOn)
                .ToListAsync();
        }

        public async Task<bool> IsFavoriteAsync(string userId, int movieId)
        {
            return await _favoriteRepository.Query()
                .AnyAsync(f => f.UserId == userId && f.MovieId == movieId);
        }

       
        public async Task<bool> ToggleAsync(string userId, int movieId)
        {
            var existing = await _favoriteRepository.Query()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.MovieId == movieId);

            if (existing != null)
            {
                _favoriteRepository.Remove(existing);
                await _favoriteRepository.SaveChangesAsync();
                return false;
            }

            await _favoriteRepository.AddAsync(new Favorite { UserId = userId, MovieId = movieId });
            await _favoriteRepository.SaveChangesAsync();
            return true;
        }
    }
}

using MvC_SystemMovies.Models;

namespace MvC_SystemMovies.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<List<Favorite>> GetFavoritesAsync(string userId);
        Task<bool> IsFavoriteAsync(string userId, int movieId);
        Task<bool> ToggleAsync(string userId, int movieId);
    }
}

using MvC_SystemMovies.Models;

namespace MvC_SystemMovies.Services.Interfaces
{
    public interface ICartService
    {
        Task<List<CartItem>> GetCartAsync(string userId);
        Task AddToCartAsync(string userId, int movieId, int quantity = 1);
        Task UpdateQuantityAsync(string userId, int cartItemId, int quantity);
        Task RemoveAsync(string userId, int cartItemId);
        Task ClearAsync(string userId);
        Task<decimal> GetTotalAsync(string userId);
        Task<int> GetItemCountAsync(string userId);
    }
}

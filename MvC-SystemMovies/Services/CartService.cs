using Microsoft.EntityFrameworkCore;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Repositories.Interfaces;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<CartItem> _cartRepository;

        public CartService(IRepository<CartItem> cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<List<CartItem>> GetCartAsync(string userId)
        {
            return await _cartRepository.Query()
                .Include(c => c.Movie)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.AddedOn)
                .ToListAsync();
        }

        public async Task AddToCartAsync(string userId, int movieId, int quantity = 1)
        {
            var existing = await _cartRepository.Query()
                .FirstOrDefaultAsync(c => c.UserId == userId && c.MovieId == movieId);

            if (existing != null)
            {
                existing.Quantity += quantity;
                _cartRepository.Update(existing);
            }
            else
            {
                await _cartRepository.AddAsync(new CartItem
                {
                    UserId = userId,
                    MovieId = movieId,
                    Quantity = quantity
                });
            }

            await _cartRepository.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(string userId, int cartItemId, int quantity)
        {
            var item = await _cartRepository.Query()
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

            if (item == null) return;

            if (quantity <= 0)
            {
                _cartRepository.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
                _cartRepository.Update(item);
            }

            await _cartRepository.SaveChangesAsync();
        }

        public async Task RemoveAsync(string userId, int cartItemId)
        {
            var item = await _cartRepository.Query()
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

            if (item == null) return;

            _cartRepository.Remove(item);
            await _cartRepository.SaveChangesAsync();
        }

        public async Task ClearAsync(string userId)
        {
            var items = await _cartRepository.Query().Where(c => c.UserId == userId).ToListAsync();
            foreach (var item in items)
                _cartRepository.Remove(item);

            await _cartRepository.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalAsync(string userId)
        {
            return await _cartRepository.Query()
                .Include(c => c.Movie)
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity * (c.Movie != null ? c.Movie.Price : 0));
        }

        public async Task<int> GetItemCountAsync(string userId)
        {
            return await _cartRepository.Query().Where(c => c.UserId == userId).SumAsync(c => c.Quantity);
        }
    }
}

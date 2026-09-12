using MvC_SystemMovies.Models;

namespace MvC_SystemMovies.Services.Interfaces
{
    public class CheckoutRequest
    {
        public int? ShippingCompanyId { get; set; }
        public string? ShippingAddress { get; set; }
        public PaymentRequest Payment { get; set; } = new();
    }

    public class CheckoutResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Order? Order { get; set; }
    }

    public interface IOrderService
    {
        Task<CheckoutResult> CheckoutAsync(string userId, CheckoutRequest request);
        Task<List<Order>> GetOrdersForUserAsync(string userId);
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<bool> UpdateStatusAsync(int orderId, OrderStatus status, string? trackingNumber = null);
    }
}

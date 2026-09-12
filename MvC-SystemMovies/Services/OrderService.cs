using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MvC_SystemMovies.Hubs;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Repositories.Interfaces;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ShippingCompany> _shippingRepository;
        private readonly ICartService _cartService;
        private readonly IPaymentService _paymentService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public OrderService(
            IRepository<Order> orderRepository,
            IRepository<ShippingCompany> shippingRepository,
            ICartService cartService,
            IPaymentService paymentService,
            IHubContext<NotificationHub> hubContext)
        {
            _orderRepository = orderRepository;
            _shippingRepository = shippingRepository;
            _cartService = cartService;
            _paymentService = paymentService;
            _hubContext = hubContext;
        }

        public async Task<CheckoutResult> CheckoutAsync(string userId, CheckoutRequest request)
        {
            var cartItems = await _cartService.GetCartAsync(userId);
            if (cartItems.Count == 0)
                return new CheckoutResult { Success = false, ErrorMessage = "Your cart is empty." };

            var subTotal = cartItems.Sum(c => c.Quantity * (c.Movie?.Price ?? 0));

            decimal shippingCost = 0;
            ShippingCompany? shippingCompany = null;
            if (request.ShippingCompanyId.HasValue)
            {
                shippingCompany = await _shippingRepository.GetByIdAsync(request.ShippingCompanyId.Value);
                shippingCost = shippingCompany?.Price ?? 0;
            }

            var total = subTotal + shippingCost;
            request.Payment.Amount = total;

            var paymentResult = await _paymentService.ChargeAsync(request.Payment);

            var order = new Order
            {
                UserId = userId,
                SubTotal = subTotal,
                ShippingCost = shippingCost,
                TotalAmount = total,
                ShippingCompanyId = shippingCompany?.Id,
                ShippingAddress = request.ShippingAddress,
                PaymentMethod = request.Payment.PaymentMethod,
                PaymentStatus = paymentResult.Success ? PaymentStatus.Paid : PaymentStatus.Failed,
                PaymentTransactionId = paymentResult.TransactionId,
                Status = paymentResult.Success ? OrderStatus.Confirmed : OrderStatus.Pending,
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    MovieId = c.MovieId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Movie?.Price ?? 0
                }).ToList()
            };

            if (!paymentResult.Success)
            {
                return new CheckoutResult { Success = false, ErrorMessage = paymentResult.ErrorMessage ?? "Payment failed.", Order = order };
            }

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            await _cartService.ClearAsync(userId);

           
            await _hubContext.Clients.Group("Admins").SendAsync("NewOrderPlaced", new
            {
                order.Id,
                order.TotalAmount,
                UserId = userId
            });

            return new CheckoutResult { Success = true, Order = order };
        }

        public async Task<List<Order>> GetOrdersForUserAsync(string userId)
        {
            return await _orderRepository.Query()
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Movie)
                .Include(o => o.ShippingCompany)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.Query()
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Movie)
                .Include(o => o.ShippingCompany)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _orderRepository.Query()
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Movie)
                .Include(o => o.ShippingCompany)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> UpdateStatusAsync(int orderId, OrderStatus status, string? trackingNumber = null)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            if (!string.IsNullOrWhiteSpace(trackingNumber))
                order.TrackingNumber = trackingNumber;

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            
            await _hubContext.Clients.Group($"user-{order.UserId}").SendAsync("OrderStatusChanged", new
            {
                order.Id,
                Status = status.ToString()
            });

            return true;
        }
    }
}

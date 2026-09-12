using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Repositories.Interfaces;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize] // must be logged in to have a cart
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IRepository<ShippingCompany> _shippingRepository;

        public CartController(ICartService cartService, IOrderService orderService, IRepository<ShippingCompany> shippingRepository)
        {
            _cartService = cartService;
            _orderService = orderService;
            _shippingRepository = shippingRepository;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> Index()
        {
            var items = await _cartService.GetCartAsync(UserId);
            ViewBag.Total = await _cartService.GetTotalAsync(UserId);
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int movieId, int quantity = 1)
        {
            await _cartService.AddToCartAsync(UserId, movieId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            await _cartService.UpdateQuantityAsync(UserId, cartItemId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            await _cartService.RemoveAsync(UserId, cartItemId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Checkout()
        {
            var items = await _cartService.GetCartAsync(UserId);
            if (items.Count == 0)
                return RedirectToAction(nameof(Index));

            ViewBag.Total = await _cartService.GetTotalAsync(UserId);
            ViewBag.ShippingCompanies = (await _shippingRepository.GetAllAsync())
                .Where(s => s.IsActive).ToList();
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int? shippingCompanyId, string? shippingAddress, string paymentMethod, string? cardNumber)
        {
            var request = new CheckoutRequest
            {
                ShippingCompanyId = shippingCompanyId,
                ShippingAddress = shippingAddress,
                Payment = new PaymentRequest
                {
                    PaymentMethod = paymentMethod,
                    CardNumber = cardNumber
                }
            };

            var result = await _orderService.CheckoutAsync(UserId, request);

            if (!result.Success)
            {
                TempData["Error"] = result.ErrorMessage;
                return RedirectToAction(nameof(Checkout));
            }

            TempData["Success"] = $"Order #{result.Order!.Id} placed successfully!";
            return RedirectToAction("Details", "Order", new { id = result.Order!.Id });
        }
    }
}

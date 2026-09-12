using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> Index()
        {
            var favorites = await _favoriteService.GetFavoritesAsync(UserId);
            return View(favorites);
        }

        public class ToggleRequest
        {
            public int MovieId { get; set; }
        }

        // Called via fetch() from the movie cards / details page.
        [HttpPost]
        public async Task<IActionResult> Toggle([FromBody] ToggleRequest request)
        {
            var isFavorite = await _favoriteService.ToggleAsync(UserId, request.MovieId);
            return Json(new { isFavorite });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index()
        {
            var reviews = await _reviewService.GetAllAsync();
            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _reviewService.DeleteAsync(id);
            TempData["Success"] = "Review deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}

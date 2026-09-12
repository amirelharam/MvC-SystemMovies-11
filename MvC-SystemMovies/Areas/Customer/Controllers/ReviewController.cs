using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int movieId, int rating, string? comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (!await _reviewService.HasUserReviewedAsync(userId, movieId) && rating is >= 1 and <= 5)
            {
                await _reviewService.AddAsync(new Review
                {
                    MovieId = movieId,
                    UserId = userId,
                    Rating = rating,
                    Comment = comment
                });
            }

            return RedirectToAction("Details", "Movie", new { id = movieId });
        }
    }
}

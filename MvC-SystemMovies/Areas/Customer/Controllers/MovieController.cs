using Microsoft.AspNetCore.Mvc;
using MvC_SystemMovies.Repositories.Interfaces;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IReviewService _reviewService;

        public MovieController(
            IMovieService movieService,
            IRepository<Category> categoryRepository,
            IRepository<Cinema> cinemaRepository,
            IReviewService reviewService)
        {
            _movieService = movieService;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index(string? search, string? category, string? cinema, int page = 1)
        {
            var result = await _movieService.SearchAsync(search, category, cinema, page);

            ViewBag.Categories = await _categoryRepository.GetAllAsync();
            ViewBag.Cinemas = await _cinemaRepository.GetAllAsync();

            ViewBag.CurrentPage = result.CurrentPage;
            ViewBag.TotalPages = result.TotalPages;

            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.Cinema = cinema;

            return View(result.Movies);
        }

        // GET: /Movies/{id}/{title?}  (see the "movieDetails" custom route in Program.cs)
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieService.GetByIdAsync(id);
            if (movie == null) return NotFound();

            ViewBag.Reviews = await _reviewService.GetForMovieAsync(id);
            ViewBag.AverageRating = await _reviewService.GetAverageRatingAsync(id);

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                ViewBag.HasReviewed = userId != null && await _reviewService.HasUserReviewedAsync(userId, id);
            }
            else
            {
                ViewBag.HasReviewed = false;
            }

            return View(movie);
        }
    }
}

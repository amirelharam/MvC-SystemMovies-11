using Microsoft.AspNetCore.Mvc;
using MvC_SystemMovies.data;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Repositories.Interfaces;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;

        public MovieController(
            IMovieService movieService,
            IRepository<Category> categoryRepository,
            IRepository<Cinema> cinemaRepository)
        {
            _movieService = movieService;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.GetAllWithDetailsAsync();
            return View(movies);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, IFormFile? File)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(movie);
            }

            try
            {
                await _movieService.CreateAsync(movie, File);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(File), ex.Message);
                await PopulateDropdownsAsync();
                return View(movie);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _movieService.GetByIdAsync(id);

            if (movie == null)
                return NotFound();

            await PopulateDropdownsAsync();
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Movie movie, IFormFile? File)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(movie);
            }

            try
            {
                
                var updated = await _movieService.UpdateAsync(movie, File);
                if (!updated)
                    return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(File), ex.Message);
                await PopulateDropdownsAsync();
                return View(movie);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _movieService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync()
        {
            ViewBag.Categories = await _categoryRepository.GetAllAsync();
            ViewBag.Cinemas = await _cinemaRepository.GetAllAsync();
        }
    }
}

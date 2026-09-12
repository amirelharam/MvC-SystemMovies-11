using Microsoft.AspNetCore.Mvc;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        
        private readonly ICinemaService _cinemaService;

        public CinemaController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        public async Task<IActionResult> Index()
        {
            var cinemas = await _cinemaService.GetAllAsync();
            return View(cinemas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile? File)
        {
            if (!ModelState.IsValid)
                return View(cinema);

            try
            {
                await _cinemaService.CreateAsync(cinema, File);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(File), ex.Message);
                return View(cinema);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _cinemaService.GetByIdAsync(id);

            if (cinema == null)
                return NotFound();

            return View(cinema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile? File)
        {
            if (!ModelState.IsValid)
                return View(cinema);

            try
            {
                var updated = await _cinemaService.UpdateAsync(cinema, File);
                if (!updated)
                    return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(File), ex.Message);
                return View(cinema);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _cinemaService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}

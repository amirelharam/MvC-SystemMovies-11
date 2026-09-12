using Microsoft.AspNetCore.Mvc;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Areas.Customer.Controllers
{
    [Area("Customer")]
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
    }
}

using Microsoft.AspNetCore.Mvc;

namespace MvC_SystemMovies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashbordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

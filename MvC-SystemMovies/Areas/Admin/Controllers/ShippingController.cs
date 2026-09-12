using Microsoft.AspNetCore.Mvc;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Repositories.Interfaces;

namespace MvC_SystemMovies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShippingController : Controller
    {
        private readonly IRepository<ShippingCompany> _shippingRepository;

        public ShippingController(IRepository<ShippingCompany> shippingRepository)
        {
            _shippingRepository = shippingRepository;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _shippingRepository.GetAllAsync();
            return View(companies.ToList());
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShippingCompany company)
        {
            if (!ModelState.IsValid) return View(company);

            await _shippingRepository.AddAsync(company);
            await _shippingRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var company = await _shippingRepository.GetByIdAsync(id);
            if (company == null) return NotFound();
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ShippingCompany company)
        {
            if (!ModelState.IsValid) return View(company);

            var existing = await _shippingRepository.GetByIdAsync(company.Id);
            if (existing == null) return NotFound();

            existing.Name = company.Name;
            existing.Price = company.Price;
            existing.EstimatedDays = company.EstimatedDays;
            existing.IsActive = company.IsActive;

            _shippingRepository.Update(existing);
            await _shippingRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await _shippingRepository.GetByIdAsync(id);
            if (company == null) return NotFound();

            _shippingRepository.Remove(company);
            await _shippingRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

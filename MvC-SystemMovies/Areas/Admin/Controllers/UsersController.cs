using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvC_SystemMovies.Areas.Admin.Models;
using MvC_SystemMovies.data;
using MvC_SystemMovies.Models;

namespace MvC_SystemMovies.Areas.Admin.Controllers
{
    [Area("Admin")]
   
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var list = new List<UserListItemViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                list.Add(new UserListItemViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    IsBlocked = user.IsBlocked,
                    Roles = roles
                });
            }

            return View(list);
        }

        
        public async Task<IActionResult> ChangeRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name!).ToList();

            var vm = new ChangeRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                CurrentRole = currentRoles.FirstOrDefault() ?? string.Empty,
                SelectedRole = currentRoles.FirstOrDefault() ?? string.Empty,
                AvailableRoles = allRoles
            };

            return View(vm);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(ChangeRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);

            
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrEmpty(model.SelectedRole))
                await _userManager.AddToRoleAsync(user, model.SelectedRole);

            TempData["Success"] = $"Role for {user.Email} updated to {model.SelectedRole}.";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsBlocked = true;
            user.LockoutEnabled = true;
           
            user.LockoutEnd = DateTimeOffset.MaxValue;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = $"{user.Email} has been blocked.";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsBlocked = false;
            user.LockoutEnd = null;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = $"{user.Email} has been unblocked.";
            return RedirectToAction(nameof(Index));
        }
    }
}

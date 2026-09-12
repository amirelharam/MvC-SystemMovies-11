using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MvC_SystemMovies.Areas.Identity.Pages.Account
{
    public class RegisterConfirmationModel : PageModel
    {
        public string Email { get; set; } = string.Empty;

        public void OnGet(string email, string? returnUrl = null)
        {
            Email = email;
        }
    }
}

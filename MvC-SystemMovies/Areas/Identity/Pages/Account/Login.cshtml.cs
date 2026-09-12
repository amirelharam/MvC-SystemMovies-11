using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MvC_SystemMovies.Models;

namespace MvC_SystemMovies.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        [TempData]
        public string? ErrorMessage { get; set; }

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required, DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                ModelState.AddModelError(string.Empty, ErrorMessage);

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // Explicit Block/Unblock done by an Admin (see UsersController.Block) sets
            // LockoutEnabled + LockoutEnd, so a blocked user is rejected right here too.
            if (user.IsBlocked)
            {
                ModelState.AddModelError(string.Empty, "Your account has been blocked by an administrator.");
                return Page();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // At this point the password is correct: send an OTP code and finish
            // the sign-in on the VerifyOtp page (extra layer on top of Identity's
            // own email-confirmation / 2FA).
            HttpContext.Session.SetString("2fa-user-id", user.Id);
            HttpContext.Session.SetString("2fa-remember-me", Input.RememberMe.ToString());
            HttpContext.Session.SetString("2fa-return-url", returnUrl);

            return RedirectToPage("./VerifyOtp");
        }
    }
}

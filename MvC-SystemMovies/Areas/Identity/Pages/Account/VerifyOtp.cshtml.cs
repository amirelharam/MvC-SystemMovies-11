using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Areas.Identity.Pages.Account
{
    public class VerifyOtpModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOtpService _otpService;

        public VerifyOtpModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOtpService otpService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _otpService = otpService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public class InputModel
        {
            [Required, Display(Name = "OTP code")]
            public string Code { get; set; } = string.Empty;
        }

        private async Task<ApplicationUser?> GetPendingUserAsync()
        {
            var userId = HttpContext.Session.GetString("2fa-user-id");
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await GetPendingUserAsync();
            if (user is null)
                return RedirectToPage("./Login");

            await _otpService.GenerateAndSendOtpAsync(user);
            StatusMessage = $"A verification code was sent to {user.Email}.";
            return Page();
        }

        public async Task<IActionResult> OnPostResendAsync()
        {
            var user = await GetPendingUserAsync();
            if (user is null)
                return RedirectToPage("./Login");

            await _otpService.GenerateAndSendOtpAsync(user);
            StatusMessage = "A new code was sent to your email.";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await GetPendingUserAsync();
            if (user is null)
                return RedirectToPage("./Login");

            if (!ModelState.IsValid)
                return Page();

            if (!_otpService.ValidateOtp(user, Input.Code))
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired code.");
                return Page();
            }

            var rememberMe = HttpContext.Session.GetString("2fa-remember-me") == "True";
            var returnUrl = HttpContext.Session.GetString("2fa-return-url") ?? Url.Content("~/");

            // Clear the used code and finish signing the user in for real.
            user.OtpCode = null;
            user.OtpCodeExpiresOn = null;
            await _userManager.UpdateAsync(user);

            await _signInManager.SignInAsync(user, isPersistent: rememberMe);

            HttpContext.Session.Remove("2fa-user-id");
            HttpContext.Session.Remove("2fa-remember-me");
            HttpContext.Session.Remove("2fa-return-url");

            return LocalRedirect(returnUrl);
        }
    }
}

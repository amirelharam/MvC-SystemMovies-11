using Microsoft.AspNetCore.Identity;
using MvC_SystemMovies.Models;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Services
{
    public class OtpService : IOtpService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public OtpService(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<string> GenerateAndSendOtpAsync(ApplicationUser user)
        {
            var code = Random.Shared.Next(100000, 999999).ToString();

            user.OtpCode = code;
            user.OtpCodeExpiresOn = DateTime.Now.AddMinutes(5);
            await _userManager.UpdateAsync(user);

            var body = $@"
                <h3>System Movies - Verification Code</h3>
                <p>Your one-time code is:</p>
                <h2 style='letter-spacing:4px'>{code}</h2>
                <p>This code expires in 5 minutes. If you did not request it, ignore this email.</p>";

            await _emailSender.SendEmailAsync(user.Email!, "Your OTP code - System Movies", body);

            return code;
        }

        public bool ValidateOtp(ApplicationUser user, string code)
        {
            if (string.IsNullOrEmpty(user.OtpCode) || user.OtpCodeExpiresOn is null)
                return false;

            if (user.OtpCodeExpiresOn < DateTime.Now)
                return false;

            return user.OtpCode == code;
        }
    }
}

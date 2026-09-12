using MvC_SystemMovies.Models;

namespace MvC_SystemMovies.Services.Interfaces
{
    public interface IOtpService
    {
        
        Task<string> GenerateAndSendOtpAsync(ApplicationUser user);

        
        bool ValidateOtp(ApplicationUser user, string code);
    }
}

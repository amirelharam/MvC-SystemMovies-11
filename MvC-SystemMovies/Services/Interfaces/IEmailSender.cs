namespace MvC_SystemMovies.Services.Interfaces
{
    /// <summary>
    /// Abstraction over the actual mail transport (SMTP here) so it is used the
    /// same way for: email confirmation, forgot password, and OTP codes.
    /// </summary>
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }
}

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Services
{
   
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string SenderName { get; set; } = "System Movies";
        public string SenderEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }

    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<SmtpSettings> settings, ILogger<EmailSender> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                using var client = new SmtpClient(_settings.Host, _settings.Port)
                {
                    Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                    EnableSsl = _settings.EnableSsl
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                message.To.Add(toEmail);

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            }
        }
    }
}

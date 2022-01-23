using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Mail
{
    public class EmailService : IEmailService
    {
        public EmailSettings _emailSettings { get; }
        public ILogger<EmailService> _logger { get; }

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendMail(Email mail)
        {
            var client = new SendGridClient(_emailSettings.ApiKey);

            var subject = mail.Subject;
            var to = new EmailAddress(mail.To);
            var emailBody = mail.Body;

            var from = new EmailAddress { Email = _emailSettings.FromAddress, Name = _emailSettings.FromName };

            var sendGridMessage = MailHelper.CreateSingleEmail(from, to, subject, emailBody, emailBody);
            var response = await client.SendEmailAsync(sendGridMessage);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted ||
                response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("Mail Sent.");
                return true;
            }

            _logger.LogInformation("Mail Sending failed.");
            return false;
        }
    }
}

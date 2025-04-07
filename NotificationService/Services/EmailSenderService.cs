using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NotificationService.Models;

namespace NotificationService.Services
{
    public class EmailSenderService
    {
        private readonly IConfiguration _configuration;

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                _configuration["SmtpSettings:SenderName"],
                _configuration["SmtpSettings:SenderEmail"]
            ));
            email.To.Add(MailboxAddress.Parse(message.To));
            email.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = message.Body };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(_configuration["SmtpSettings:Host"],
                                    int.Parse(_configuration["SmtpSettings:Port"]),
                                    SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(
                _configuration["SmtpSettings:Username"],
                _configuration["SmtpSettings:Password"]
            );
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}

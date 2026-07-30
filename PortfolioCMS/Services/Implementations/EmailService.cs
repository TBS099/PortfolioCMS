using System.Net;
using System.Net.Mail;

namespace PortfolioCMS.Services.Implementations
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var smtpHost = emailSettings["SmtpHost"]!;
            var smtpPort = int.Parse(emailSettings["SmtpPort"]!);
            var senderEmail = emailSettings["SenderEmail"]!;
            var senderName = emailSettings["SenderName"]!;
            var password = emailSettings["Password"]!;

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = "Password Reset Request",
                Body = $@"
                    <h2>Password Reset</h2>
                    <p>Click the link below to reset your password:</p>
                    <a href='{resetLink}'>Reset Password</a>
                    <p>This link expires in 1 hour.</p>
                    <p>If you didn't request this, ignore this email.</p>
                ",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Net;

namespace PortfolioCMS.Services.Implementations
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // Shared connect/authenticate/send/disconnect sequence — both email
        // methods below build a MimeMessage and hand it here.
        private async Task SendAsync(MimeMessage mailMessage)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var smtpHost = emailSettings["SmtpHost"]!;
            var smtpPort = int.Parse(emailSettings["SmtpPort"]!);
            var senderEmail = emailSettings["SenderEmail"]!;
            var password = emailSettings["Password"]!;

            // Fully qualified: if this file (or the project) also has
            // `using System.Net.Mail;` anywhere in scope, an unqualified
            // "SmtpClient" is ambiguous between that and MailKit's — this
            // makes the choice explicit regardless of what else is imported.
            using var client = new MailKit.Net.Smtp.SmtpClient();
            // StartTls matches the previous System.Net.Mail behavior
            // (EnableSsl = true on port 587); SmtpClient will pick the right
            // handshake for 465 (implicit TLS) automatically if you ever
            // switch ports, via SecureSocketOptions.Auto.
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(senderEmail, password);
            await client.SendAsync(mailMessage);
            await client.DisconnectAsync(true);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var senderEmail = emailSettings["SenderEmail"]!;
            var senderName = emailSettings["SenderName"]!;

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(senderName, senderEmail));
            mailMessage.To.Add(new MailboxAddress("", toEmail));
            mailMessage.Subject = "Password Reset Request";
            mailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = $@"
                    <h2>Password Reset</h2>
                    <p>Click the link below to reset your password:</p>
                    <a href='{resetLink}'>Reset Password</a>
                    <p>This link expires in 1 hour.</p>
                    <p>If you didn't request this, ignore this email.</p>
                "
            };

            try
            {
                await SendAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send password reset email to {Email}.", toEmail);
            }
        }

        // Sends a contact-form submission to the site owner (SenderEmail),
        // with the visitor's address set as ReplyTo so replying from your
        // inbox goes straight back to them. Unlike the password-reset email
        // above, failures here are NOT swallowed — the controller needs to
        // know if delivery failed so it can return an error to the frontend
        // instead of silently reporting success for a message nobody got.
        public async Task SendContactMessageAsync(string fromName, string fromEmail, string message)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var senderEmail = emailSettings["SenderEmail"]!;
            var senderName = emailSettings["SenderName"]!;

            // Encode visitor-supplied values before embedding them in an HTML
            // body — this endpoint is unauthenticated and public, so treat
            // every field as untrusted input. Also strip CR/LF from the name
            // before it goes into the Subject header: MimeKit encodes headers
            // safely on its own, but there's no reason for this field to
            // legitimately contain a newline.
            var subjectSafeName = fromName.Replace("\r", "").Replace("\n", "");
            var safeName = WebUtility.HtmlEncode(fromName);
            var safeMessage = WebUtility.HtmlEncode(message).Replace("\n", "<br/>");

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(senderName, senderEmail));
            mailMessage.To.Add(new MailboxAddress("", senderEmail));
            // ReplyTo, not From — SMTP providers (Gmail included) reject or
            // flag mail whose From doesn't match the authenticated account,
            // so the visitor's address goes here instead. Hitting "reply" in
            // your inbox lands on their address, exactly like a normal email.
            mailMessage.ReplyTo.Add(new MailboxAddress(fromName, fromEmail));
            mailMessage.Subject = $"Portfolio contact form: {subjectSafeName}";
            mailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = $@"
                    <h2>New contact form submission</h2>
                    <p><strong>Name:</strong> {safeName}</p>
                    <p><strong>Email:</strong> {WebUtility.HtmlEncode(fromEmail)}</p>
                    <p><strong>Message:</strong></p>
                    <p>{safeMessage}</p>
                "
            };

            await SendAsync(mailMessage);
        }
    }
}
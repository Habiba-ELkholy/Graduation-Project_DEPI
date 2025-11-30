using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;


namespace c2cUniversitees.Utilities
{

    
        public interface IEmailSender
        {
            Task SendEmailAsync(string toEmail, string subject, string message);
        }

        public class EmailSender : IEmailSender
        {
            private readonly EmailSettings _emailSettings;

            public EmailSender(IOptions<EmailSettings> emailSettings)
            {
                _emailSettings = emailSettings.Value;
            }

            public async Task SendEmailAsync(string toEmail, string subject, string message)
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_emailSettings.SenderEmail);
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = message
                };
                email.Body = builder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                try
                {
                    await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);

                    await smtp.SendAsync(email);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"فشل إرسال البريد: {ex.Message}", ex);
                }
                finally
                {
                    await smtp.DisconnectAsync(true);
                }
            }
        }

    }

    



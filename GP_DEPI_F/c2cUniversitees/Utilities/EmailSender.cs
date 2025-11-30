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

        // 2. تعريف الكلاس الذي يُنفذ الواجهة
        public class EmailSender : IEmailSender
        {
            private readonly EmailSettings _emailSettings;

            // Constructor
            public EmailSender(IOptions<EmailSettings> emailSettings)
            {
                _emailSettings = emailSettings.Value;
            }

            public async Task SendEmailAsync(string toEmail, string subject, string message)
            {
                // بناء محتوى الرسالة باستخدام MimeKit
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_emailSettings.SenderEmail);
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = message
                };
                email.Body = builder.ToMessageBody();

                // استخدام SmtpClient من مكتبة MailKit.Net.Smtp
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                // أو فقط: using var smtp = new SmtpClient(); إذا حذفنا using System.Net.Mail;

                try
                {
                    // جميع هذه الدوال خاصة بـ MailKit وتعمل الآن:
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

    



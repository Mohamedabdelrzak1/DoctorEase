using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Service.Settings;
using Shared.Dtos;
using System;

namespace Service.Helpers
{
    public class MailingService : IMailingService
    {
        

        private readonly MailSettings _mailSettings;

        public MailingService(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }
        public void SendEmail(Email email)
        {
            try
            {
                // Build the message
                var mail = new MimeMessage();
                mail.Subject = email.Subject;
                mail.From.Add(MailboxAddress.Parse(_mailSettings.Email));
                mail.To.Add(MailboxAddress.Parse(email.To));

                var builder = new BodyBuilder
                {
                    TextBody = email.Body
                    // If you want HTML support:
                    // HtmlBody = email.Body
                };

                mail.Body = builder.ToMessageBody();

                // Establish connection and send
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);

                smtp.Send(mail);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                // Log or rethrow with clear context
                throw new Exception($"❌ Failed to send email to {email.To}: {ex.Message}", ex);
            }
        }
    }
}

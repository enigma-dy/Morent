using System.Net;
using System.Net.Mail;

namespace MoRent_V2.Services;

public class EmailService(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, bool enableSsl)
{
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(smtpServer, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = enableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpUsername),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage);
    }
}

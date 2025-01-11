using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace eBookLibrary.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpHost = "smtp.gmail.com"; // Change to your SMTP server
        private readonly int _smtpPort = 587; // Default port for Gmail SMTP
        private readonly string _smtpUser = "your-email@gmail.com"; // Replace with your email
        private readonly string _smtpPass = "your-password"; // Replace with your email password

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var smtpClient = new SmtpClient(_smtpHost))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUser),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}

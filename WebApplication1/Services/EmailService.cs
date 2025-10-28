using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System.Net.Mail;

namespace TicketBookingWebAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var mailSettings = _config.GetSection("MailSettings");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                mailSettings["FromName"],
                mailSettings["FromEmail"]
            ));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(mailSettings["Host"], int.Parse(mailSettings["Port"]), SecureSocketOptions.StartTls);
            smtp.Authenticate(mailSettings["Username"], mailSettings["Password"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
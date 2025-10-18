using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace TicketBooking_BusinessDataLogic
{
    class EmailService
    {
        public void SendBookingConfirmation(string userEmail, string userName, string movieTitle, int seats, DateTime showTime)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Movie Booking", "do-not-reply@moviebooking.com"));
            message.To.Add(new MailboxAddress(userName, userEmail));
            message.Subject = "Movie Ticket Booking Confirmation";

            message.Body = new TextPart("plain")
            {
                Text = $"Hello {userName},\n\n" +
                       $"Your booking has been confirmed!\n\n" +
                       $"Movie: {movieTitle}\n" +
                       $"Seats: {seats}\n" +
                       $"Show Time: {showTime}\n\n" +
                       "Enjoy your movie!\n\n" +
                       "- Movie Booking Team"
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    var smtpHost = "sandbox.smtp.mailtrap.io";
                    var smtpPort = 2525;
                    var username = "16a25efa3bbce1";
                    var password = "e9757c0cd0ff0c"; 

                    client.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                    client.Authenticate(username, password);
                    client.Send(message);
                    client.Disconnect(true);

                    Console.WriteLine("Email confirmation sent to " + userEmail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send email: " + ex.Message);
                }
            }
        }
    }
}

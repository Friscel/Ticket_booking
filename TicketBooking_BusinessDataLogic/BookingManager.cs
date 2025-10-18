using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingCommon;
using TicketBookingDataService;

namespace TicketBooking_BusinessDataLogic
{
    class BookingManager
    {
        private readonly MovieDataService movieService = new MovieDataService();
        private readonly EmailService emailService = new EmailService();

        public void BookMovie(User user, Movie selectedMovie, int seatsToBook)
        {
            if (selectedMovie.AvailableTickets >= seatsToBook)
            {
                selectedMovie.BookedTickets += seatsToBook;
                selectedMovie.AvailableTickets -= seatsToBook;

                movieService.UpdateMovie(selectedMovie);

                Console.WriteLine("Booking successful!");

                emailService.SendBookingConfirmation(
                    user.Email,
                    user.Username,
                    selectedMovie.Title,
                    seatsToBook,
                    DateTime.Now
                );
            }
            else
            {
                Console.WriteLine("Not enough tickets available.");
            }
        }
    }
}

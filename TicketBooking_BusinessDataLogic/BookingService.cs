using TicketBookingDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace TicketBooking_BusinessDataLogic
{
    public class BookingService
    {
        private MovieDataService movieDataService = new MovieDataService();

        public string[] GetMovies()
        {
            return movieDataService.GetMovieTitles();
        }

        public int[] GetAvailableTickets()
        {
            return movieDataService.GetAvailableTicketCounts();
        }

        // Method to check ticket availability
        public bool CheckTicketAvailability(int movieChoice, int amount)
        {
            if (amount <= 0)
                return false;

            int availableTickets = movieDataService.GetAvailableTicketsForMovie(movieChoice);
            return availableTickets >= amount;
        }

        // Method to update tickets based on action
        public bool UpdateTickets(Actions action, int movieChoice, int amount)
        {
            int availableTickets = movieDataService.GetAvailableTicketsForMovie(movieChoice);
            int bookedTickets = movieDataService.GetBookedTicketsForMovie(movieChoice);

            if (availableTickets < 0 || bookedTickets < 0)
                return false;

            if (action == Actions.BookTicket)
            {
                if (availableTickets >= amount)
                {
                    availableTickets -= amount;
                    bookedTickets += amount;
                    return movieDataService.UpdateMovieTickets(movieChoice, availableTickets, bookedTickets);
                }
            }
            else if (action == Actions.CancelTicket)
            {
                if (bookedTickets >= amount)
                {
                    availableTickets += amount;
                    bookedTickets -= amount;
                    return movieDataService.UpdateMovieTickets(movieChoice, availableTickets, bookedTickets);
                }
            }

            return false;
        }

        // Method to get available tickets for a movie
        public int GetAvailableTicketsForMovie(int movieChoice)
        {
            return movieDataService.GetAvailableTicketsForMovie(movieChoice);
        }

        // Method to get booked tickets for a movie
        public int GetBookedTicketsForMovie(int movieChoice)
        {
            return movieDataService.GetBookedTicketsForMovie(movieChoice);
        }

        // Method to search for movies
        public System.Collections.Generic.List<TicketBookingCommon.Movie> SearchMovies(string searchTerm)
        {
            return movieDataService.SearchMoviesByTitle(searchTerm);
        }

        // Method to add a new movie - Admin only
        public bool AddMovie(string title, int availableTickets)
        {
            return movieDataService.AddMovie(title, availableTickets);
        }

        // Method to delete a movie - Admin only
        public bool DeleteMovie(int movieIndex)
        {
            return movieDataService.DeleteMovie(movieIndex);
        }
    }
}
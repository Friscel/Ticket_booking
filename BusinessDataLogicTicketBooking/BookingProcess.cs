using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessDataLogicTicketBooking
{
    public class BookingProcess
    {
        public static string[] movies = { "Avengers: Endgame", "Inception", "Interstellar", "Joker" };
        public static int[] availableTickets = { 55, 40, 30, 20 };
        public static int[] bookedTickets = { 0, 0, 0, 0 };
        public static string[] GetMovies()
        {
            return movies;
        }
        public static int[] GetAvailableTickets()
        {
            return availableTickets;
        }
        // Method to update tickets based on action
        public static bool UpdateTickets(Actions action, int movieChoice, int amount)
        {
            if (movieChoice < 0 || movieChoice >= movies.Length)
            {
                return false;
            }

            if (action == Actions.BookTicket)
            {
                if (availableTickets[movieChoice] >= amount)
                {
                    availableTickets[movieChoice] -= amount;
                    bookedTickets[movieChoice] += amount;
                    return true;
                }
                return false;
            }

            if (action == Actions.CancelTicket)
            {
                if (bookedTickets[movieChoice] >= amount)
                {
                    availableTickets[movieChoice] += amount;
                    bookedTickets[movieChoice] -= amount;
                    return true;
                }
                return false;
            }

            return false;
        }
        // Method to check ticket availability
        public static bool CheckTicketAvailability(int movieChoice, int amount)
        {
            if (movieChoice < 0 || movieChoice >= movies.Length || amount <= 0)
            {
                return false;
            }

            return availableTickets[movieChoice] >= amount;
        }
        // Method to get current available tickets for a movie
        public static int GetAvailableTicketsForMovie(int movieChoice)
        {
            if (movieChoice < 0 || movieChoice >= movies.Length)
            {
                return -1;
            }
            return availableTickets[movieChoice];
        }
        // Method to get booked tickets for a movie
        public static int GetBookedTicketsForMovie(int movieChoice)
        {
            if (movieChoice < 0 || movieChoice >= movies.Length)
            {
                return -1;
            }
            return bookedTickets[movieChoice];
        }
    }
}
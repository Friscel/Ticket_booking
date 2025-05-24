using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingCommon;
using TicketBookingDataService;

namespace TicketBooking_BusinessDataLogic
{
    public class MovieService
    {
        private MovieDataService movieDataService = new MovieDataService();

        public string[] GetMovies()
        {
            var movies = movieDataService.GetAllMovies();
            return movies.Select(m => m.Title).ToArray();
        }

        public int[] GetAvailableTickets()
        {
            var movies = movieDataService.GetAllMovies();
            return movies.Select(m => m.AvailableTickets).ToArray();
        }

        // Method to check ticket availability
        public bool CheckTicketAvailability(int movieChoice, int amount)
        {
            if (amount <= 0) return false;

            var movies = movieDataService.GetAllMovies();
            if (movieChoice >= 0 && movieChoice < movies.Count)
            {
                return movies[movieChoice].AvailableTickets >= amount;
            }
            return false;
        }

        // Method to update tickets based on action
        public bool UpdateTickets(Actions action, int movieChoice, int amount)
        {
            var movies = movieDataService.GetAllMovies();
            if (movieChoice < 0 || movieChoice >= movies.Count) return false;

            var movie = movies[movieChoice];
            var updatedMovie = new Movie
            {
                Title = movie.Title,
                AvailableTickets = movie.AvailableTickets,
                BookedTickets = movie.BookedTickets
            };

            if (action == Actions.BookTicket)
            {
                if (movie.AvailableTickets >= amount)
                {
                    updatedMovie.AvailableTickets -= amount;
                    updatedMovie.BookedTickets += amount;
                    movieDataService.UpdateMovie(updatedMovie);
                    return true;
                }
            }
            else if (action == Actions.CancelTicket)
            {
                if (movie.BookedTickets >= amount)
                {
                    updatedMovie.AvailableTickets += amount;
                    updatedMovie.BookedTickets -= amount;
                    movieDataService.UpdateMovie(updatedMovie);
                    return true;
                }
            }

            return false;
        }

        // Method to get available tickets for a movie
        public int GetAvailableTicketsForMovie(int movieChoice)
        {
            var movies = movieDataService.GetAllMovies();
            if (movieChoice >= 0 && movieChoice < movies.Count)
            {
                return movies[movieChoice].AvailableTickets;
            }
            return -1;
        }

        // Method to get booked tickets for a movie
        public int GetBookedTicketsForMovie(int movieChoice)
        {
            var movies = movieDataService.GetAllMovies();
            if (movieChoice >= 0 && movieChoice < movies.Count)
            {
                return movies[movieChoice].BookedTickets;
            }
            return -1;
        }

        // Method to search for movies
        public List<Movie> SearchMovies(string searchTerm)
        {
            var allMovies = movieDataService.GetAllMovies();
            return allMovies.Where(m => m.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Method to add a new movie
        public bool AddMovie(string title, int availableTickets)
        {
            if (string.IsNullOrWhiteSpace(title) || availableTickets <= 0)
                return false;

            // Check if movie already exists
            var existingMovies = movieDataService.GetAllMovies();
            if (existingMovies.Any(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
                return false;

            var newMovie = new Movie
            {
                Title = title,
                AvailableTickets = availableTickets,
                BookedTickets = 0
            };

            movieDataService.AddMovie(newMovie);
            return true;
        }

        // Method to delete a movie
        public bool DeleteMovie(int movieIndex)
        {
            var movies = movieDataService.GetAllMovies();
            if (movieIndex >= 0 && movieIndex < movies.Count)
            {
                movieDataService.RemoveMovie(movies[movieIndex]);
                return true;
            }
            return false;
        }
    }
}
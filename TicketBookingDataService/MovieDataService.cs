using TicketBookingCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace TicketBookingDataService
{
    public class MovieDataService
    {
        private List<Movie> movies = new List<Movie>();

        public MovieDataService()
        {
            CreateMovieInventory();
        }

        private void CreateMovieInventory()
        {
            movies.Add(new Movie
            {
                Title = "Avengers: Endgame",
                AvailableTickets = 55,
                BookedTickets = 0
            });

            movies.Add(new Movie
            {
                Title = "Inception",
                AvailableTickets = 40,
                BookedTickets = 0
            });

            movies.Add(new Movie
            {
                Title = "Interstellar",
                AvailableTickets = 30,
                BookedTickets = 0
            });

            movies.Add(new Movie
            {
                Title = "Joker",
                AvailableTickets = 20,
                BookedTickets = 0
            });
        }

        public string[] GetMovieTitles()
        {
            List<string> titles = new List<string>();
            foreach (var movie in movies)
            {
                titles.Add(movie.Title);
            }
            return titles.ToArray();
        }

        public int[] GetAvailableTicketCounts()
        {
            List<int> available = new List<int>();
            foreach (var movie in movies)
            {
                available.Add(movie.AvailableTickets);
            }
            return available.ToArray();
        }

        public int GetAvailableTicketsForMovie(int movieIndex)
        {
            if (movieIndex >= 0 && movieIndex < movies.Count)
            {
                return movies[movieIndex].AvailableTickets;
            }
            return -1;
        }

        public int GetBookedTicketsForMovie(int movieIndex)
        {
            if (movieIndex >= 0 && movieIndex < movies.Count)
            {
                return movies[movieIndex].BookedTickets;
            }
            return -1;
        }

        public bool UpdateMovieTickets(int movieIndex, int availableTickets, int bookedTickets)
        {
            if (movieIndex >= 0 && movieIndex < movies.Count)
            {
                movies[movieIndex].AvailableTickets = availableTickets;
                movies[movieIndex].BookedTickets = bookedTickets;
                return true;
            }
            return false;
        }

        // Search functionality
        public List<Movie> SearchMoviesByTitle(string searchTerm)
        {
            List<Movie> results = new List<Movie>();
            foreach (var movie in movies)
            {
                if (movie.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(movie);
                }
            }
            return results;
        }

        // Add a new movie
        public bool AddMovie(string title, int availableTickets)
        {
            if (string.IsNullOrWhiteSpace(title) || availableTickets <= 0)
                return false;

            // Check if movie already exists
            foreach (var movie in movies)
            {
                if (movie.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            movies.Add(new Movie
            {
                Title = title,
                AvailableTickets = availableTickets,
                BookedTickets = 0
            });
            return true;
        }

        // Delete a movie
        public bool DeleteMovie(int movieIndex)
        {
            if (movieIndex >= 0 && movieIndex < movies.Count)
            {
                movies.RemoveAt(movieIndex);
                return true;
            }
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingCommon;

namespace TicketBookingDataService
{
    public class InMemoryMovieDataService : IMovieDataService
    {
        private static List<Movie> movies = new List<Movie>();

        public InMemoryMovieDataService()
        {
            if (movies.Count == 0)
            {
                CreateDummyMovies();
            }
        }

        private void CreateDummyMovies()
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

        public List<Movie> GetMovies()
        {
            return movies;
        }

        public void CreateMovie(Movie movie)
        {
            movies.Add(movie);
        }

        public void UpdateMovie(Movie movie)
        {
            for (int i = 0; i < movies.Count; i++)
            {
                if (movies[i].Title == movie.Title)
                {
                    movies[i].AvailableTickets = movie.AvailableTickets;
                    movies[i].BookedTickets = movie.BookedTickets;
                    break;
                }
            }
        }

        public void RemoveMovie(Movie movie)
        {
            var movieToRemove = movies.FirstOrDefault(m => m.Title == movie.Title);
            if (movieToRemove != null)
            {
                movies.Remove(movieToRemove);
            }
        }
    }
}

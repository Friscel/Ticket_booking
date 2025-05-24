using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TicketBookingCommon;

namespace TicketBookingDataService
{
    public class MovieDataService
    {
        IMovieDataService movieDataService;

        public MovieDataService()
        {
            //movieDataService = new InMemoryMovieDataService();
            //movieDataService = new TextFileMovieDataService();
            //movieDataService = new JsonFileMovieDataService();
            movieDataService = new DBMovieDataService();

        }

        public List<Movie> GetAllMovies()
        {
            return movieDataService.GetMovies();
        }

        public void AddMovie(Movie movie)
        {
            movieDataService.CreateMovie(movie);
        }

        public void UpdateMovie(Movie movie)
        {
            movieDataService.UpdateMovie(movie);
        }

        public void RemoveMovie(Movie movie)
        {
            movieDataService.RemoveMovie(movie);
        }
    }
}
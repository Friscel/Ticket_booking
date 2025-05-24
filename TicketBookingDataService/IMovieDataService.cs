using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingCommon;

namespace TicketBookingDataService
{
    public interface IMovieDataService
    {
        List<Movie> GetMovies();
        void CreateMovie(Movie movie);
        void UpdateMovie(Movie movie);
        void RemoveMovie(Movie movie);
    }
}
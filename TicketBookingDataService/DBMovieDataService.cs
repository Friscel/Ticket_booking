using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingCommon;
using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;

namespace TicketBookingDataService
{
    public class DBMovieDataService : IMovieDataService
    {
        private static string connectionString =
            "Data Source=LAPTOP-HDTASQBE\\SQLEXPRESS;Initial Catalog=TicketBookingDB;Integrated Security=True;TrustServerCertificate=True;";

        private static SqlConnection sqlConnection;

        public DBMovieDataService()
        {
            sqlConnection = new SqlConnection(connectionString);
        }

        public List<Movie> GetMovies()
        {
            string selectStatement = "SELECT Id, Title, AvailableTickets, BookedTickets FROM Movies";
            SqlCommand selectCommand = new SqlCommand(selectStatement, sqlConnection);

            sqlConnection.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();

            var movies = new List<Movie>();
            while (reader.Read())
            {
                Movie movie = new Movie
                {
                    Title = reader["Title"].ToString(),
                    AvailableTickets = Convert.ToInt32(reader["AvailableTickets"]),
                    BookedTickets = Convert.ToInt32(reader["BookedTickets"])
                };
                movies.Add(movie);
            }

            sqlConnection.Close();
            return movies;
        }

        public void CreateMovie(Movie movie)
        {
            var insertStatement = "INSERT INTO Movies (Title, AvailableTickets, BookedTickets) VALUES (@Title, @AvailableTickets, @BookedTickets)";
            SqlCommand insertCommand = new SqlCommand(insertStatement, sqlConnection);

            insertCommand.Parameters.AddWithValue("@Title", movie.Title);
            insertCommand.Parameters.AddWithValue("@AvailableTickets", movie.AvailableTickets);
            insertCommand.Parameters.AddWithValue("@BookedTickets", movie.BookedTickets);

            sqlConnection.Open();
            insertCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        public void UpdateMovie(Movie movie)
        {
            var updateStatement = "UPDATE Movies SET AvailableTickets = @AvailableTickets, BookedTickets = @BookedTickets WHERE Title = @Title";
            SqlCommand updateCommand = new SqlCommand(updateStatement, sqlConnection);

            updateCommand.Parameters.AddWithValue("@AvailableTickets", movie.AvailableTickets);
            updateCommand.Parameters.AddWithValue("@BookedTickets", movie.BookedTickets);
            updateCommand.Parameters.AddWithValue("@Title", movie.Title);

            sqlConnection.Open();
            updateCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        public void RemoveMovie(Movie movie)
        {
            var deleteStatement = "DELETE FROM Movies WHERE Title = @Title";
            SqlCommand deleteCommand = new SqlCommand(deleteStatement, sqlConnection);

            deleteCommand.Parameters.AddWithValue("@Title", movie.Title);

            sqlConnection.Open();
            deleteCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
}
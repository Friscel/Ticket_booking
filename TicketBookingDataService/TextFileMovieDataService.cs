using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingCommon;

namespace TicketBookingDataService
{
    public class TextFileMovieDataService : IMovieDataService
    {
        private string filePath = "movies.txt";
        private List<Movie> movies = new List<Movie>();

        public TextFileMovieDataService()
        {
            GetDataFromFile();
        }

        private void GetDataFromFile()
        {
            if (!File.Exists(filePath))
            {
                CreateDefaultMoviesFile();
            }

            var lines = File.ReadAllLines(filePath);
            movies.Clear();

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        movies.Add(new Movie
                        {
                            Title = parts[0],
                            AvailableTickets = Convert.ToInt32(parts[1]),
                            BookedTickets = Convert.ToInt32(parts[2])
                        });
                    }
                }
            }
        }

        private void CreateDefaultMoviesFile()
        {
            var defaultMovies = new List<string>
            {
                "Avengers: Endgame|55|0",
                "Inception|40|0",
                "Interstellar|30|0",
                "Joker|20|0"
            };
            File.WriteAllLines(filePath, defaultMovies);
        }

        private void WriteDataToFile()
        {
            var lines = new string[movies.Count];
            for (int i = 0; i < movies.Count; i++)
            {
                lines[i] = $"{movies[i].Title}|{movies[i].AvailableTickets}|{movies[i].BookedTickets}";
            }
            File.WriteAllLines(filePath, lines);
        }

        private int FindMovieIndex(string title)
        {
            for (int i = 0; i < movies.Count; i++)
            {
                if (movies[i].Title == title)
                {
                    return i;
                }
            }
            return -1;
        }

        public List<Movie> GetMovies()
        {
            return movies;
        }

        public void CreateMovie(Movie movie)
        {
            movies.Add(movie);
            WriteDataToFile();
        }

        public void UpdateMovie(Movie movie)
        {
            int index = FindMovieIndex(movie.Title);
            if (index != -1)
            {
                movies[index].AvailableTickets = movie.AvailableTickets;
                movies[index].BookedTickets = movie.BookedTickets;
                WriteDataToFile();
            }
        }

        public void RemoveMovie(Movie movie)
        {
            int index = FindMovieIndex(movie.Title);
            if (index != -1)
            {
                movies.RemoveAt(index);
                WriteDataToFile();
            }
        }
    }
}
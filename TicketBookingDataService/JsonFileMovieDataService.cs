using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using TicketBookingCommon;

namespace TicketBookingDataService
{
    public class JsonFileMovieDataService : IMovieDataService
    {
        private static List<Movie> movies = new List<Movie>();
        private static string jsonFilePath = "movies.json";

        public JsonFileMovieDataService()
        {
            ReadJsonDataFromFile();
        }

        private void ReadJsonDataFromFile()
        {
            if (!File.Exists(jsonFilePath))
            {
                CreateDefaultMoviesFile();
            }

            string jsonText = File.ReadAllText(jsonFilePath);
            if (!string.IsNullOrWhiteSpace(jsonText))
            {
                movies = JsonSerializer.Deserialize<List<Movie>>(jsonText,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
        }

        private void CreateDefaultMoviesFile()
        {
            var defaultMovies = new List<Movie>
            {
                new Movie { Title = "Avengers: Endgame", AvailableTickets = 55, BookedTickets = 0 },
                new Movie { Title = "Inception", AvailableTickets = 40, BookedTickets = 0 },
                new Movie { Title = "Interstellar", AvailableTickets = 30, BookedTickets = 0 },
                new Movie { Title = "Joker", AvailableTickets = 20, BookedTickets = 0 }
            };

            movies = defaultMovies;
            WriteJsonDataToFile();
        }

        private void WriteJsonDataToFile()
        {
            string jsonString = JsonSerializer.Serialize(movies, new JsonSerializerOptions
            { WriteIndented = true });
            File.WriteAllText(jsonFilePath, jsonString);
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
            WriteJsonDataToFile();
        }

        public void UpdateMovie(Movie movie)
        {
            int index = FindMovieIndex(movie.Title);
            if (index != -1)
            {
                movies[index].AvailableTickets = movie.AvailableTickets;
                movies[index].BookedTickets = movie.BookedTickets;
                WriteJsonDataToFile();
            }
        }

        public void RemoveMovie(Movie movie)
        {
            int index = FindMovieIndex(movie.Title);
            if (index != -1)
            {
                movies.RemoveAt(index);
                WriteJsonDataToFile();
            }
        }
    }
}
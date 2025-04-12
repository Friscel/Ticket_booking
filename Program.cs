using System.Runtime.CompilerServices;
using System.Transactions;
using System;
using TicketBooking_BusinessDataLogic;

namespace TicketBooking
{
    internal class Program
    {
        static string[] actions = { "[1] View Available Tickets", "[2] Book a Ticket", "[3] Cancel a Ticket",
                                   "[4] Select Another Movie", "[5] Search Movies", "[6] Add Movie",
                                   "[7] Delete Movie", "[8] Exit" };
        static BookingService bookingService = new BookingService();

        static void Main(string[] args)
        {
            Console.WriteLine("MOVIE TICKET BOOKING SYSTEM");

            int movieChoice;
            do
            {
                movieChoice = SelectMovie();

                if (movieChoice == -1)
                    break;

                int userAction;
                do
                {
                    DisplayActions();
                    userAction = GetUserInput();

                    if (userAction == 8) // Exit
                    {
                        Console.WriteLine("Exiting program...");
                        return;
                    }

                    HandleMovieAction(userAction, movieChoice);

                } while (userAction != 4 && userAction != 8);

            } while (movieChoice != -1);
        }

        static void DisplayMovies()
        {
            Console.WriteLine("\nAvailable Movies:");
            string[] movies = bookingService.GetMovies();
            int[] availableTickets = bookingService.GetAvailableTickets();

            for (int i = 0; i < movies.Length; i++)
            {
                Console.WriteLine($"[{i + 1}] {movies[i]} - Available Tickets: {availableTickets[i]}");
            }
            Console.WriteLine("[0] Exit");
        }

        static int SelectMovie()
        {
            int choice;
            do
            {
                DisplayMovies();
                Console.Write("Select a Movie: ");
                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    if (choice == 0)
                    {
                        return -1;
                    }

                    string[] movies = bookingService.GetMovies();
                    if (choice >= 1 && choice <= movies.Length)
                    {
                        return choice - 1; // Convert to zero-based index
                    }
                }
                Console.WriteLine("Invalid selection. Please try again.");

            } while (true);
        }

        static void DisplayActions()
        {
            Console.WriteLine("\n---------------------------------------");
            Console.WriteLine("\nACTIONS:");
            foreach (var action in actions)
            {
                Console.WriteLine(action);
            }
        }

        static int GetUserInput()
        {
            Console.Write("Select Action: ");
            if (int.TryParse(Console.ReadLine(), out int input) && input >= 1 && input <= 8)
            {
                return input;
            }
            Console.WriteLine("Invalid input. Please enter a number between 1 and 8.");
            return GetUserInput();
        }

        static void HandleMovieAction(int action, int movieChoice)
        {
            switch (action)
            {
                case 1:
                    DisplayAvailableTickets(movieChoice);
                    break;
                case 2:
                    BookTicket(movieChoice);
                    break;
                case 3:
                    CancelTicket(movieChoice);
                    break;
                case 4:
                    Console.WriteLine("Returning to movie selection...");
                    break;
                case 5:
                    SearchMovies();
                    break;
                case 6:
                    AddMovie();
                    break;
                case 7:
                    DeleteMovie();
                    break;
                case 8:
                    Console.WriteLine("Exiting...");
                    break;
            }
        }

        static void DisplayAvailableTickets(int movieChoice)
        {
            string[] movies = bookingService.GetMovies();
            Console.WriteLine($"Available tickets for {movies[movieChoice]}: {bookingService.GetAvailableTicketsForMovie(movieChoice)}");
        }

        static void BookTicket(int movieChoice)
        {
            int availableTickets = bookingService.GetAvailableTicketsForMovie(movieChoice);

            if (availableTickets == 0)
            {
                Console.WriteLine("Sorry, no tickets available for this movie.");
                return;
            }

            Console.WriteLine($"Available tickets: {availableTickets}");
            Console.Write("Enter number of tickets to book: ");

            if (int.TryParse(Console.ReadLine(), out int numTickets) && numTickets > 0)
            {
                if (numTickets > availableTickets)
                {
                    Console.WriteLine($"Sorry, only {availableTickets} tickets are available.");
                    return;
                }

                if (bookingService.CheckTicketAvailability(movieChoice, numTickets))
                {
                    if (bookingService.UpdateTickets(Actions.BookTicket, movieChoice, numTickets))
                    {
                        Console.WriteLine("Ticket booked successfully.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid number. Please enter a valid amount.");
            }

            DisplayAvailableTickets(movieChoice);
        }

        static void CancelTicket(int movieChoice)
        {
            int bookedTickets = bookingService.GetBookedTicketsForMovie(movieChoice);

            if (bookedTickets == 0)
            {
                Console.WriteLine("You have no booked tickets for this movie.");
                return;
            }

            Console.WriteLine($"You have {bookedTickets} booked tickets for this movie.");
            Console.Write("Enter number of tickets to cancel: ");

            if (int.TryParse(Console.ReadLine(), out int numTickets) && numTickets > 0)
            {
                if (numTickets > bookedTickets)
                {
                    Console.WriteLine($"You can only cancel up to {bookedTickets} tickets.");
                    return;
                }

                if (bookingService.UpdateTickets(Actions.CancelTicket, movieChoice, numTickets))
                {
                    Console.WriteLine("Ticket canceled successfully.");
                }
                else
                {
                    Console.WriteLine("Ticket cancellation failed.");
                }
            }
            else
            {
                Console.WriteLine("Invalid number. Please enter a valid amount.");
            }

            DisplayAvailableTickets(movieChoice);
        }

        // CRUD functionality
        static void SearchMovies()
        {
            Console.Write("Enter movie title to search: ");
            string searchTerm = Console.ReadLine();

            var results = bookingService.SearchMovies(searchTerm);

            if (results.Count == 0)
            {
                Console.WriteLine("No movies found matching your search.");
                return;
            }

            Console.WriteLine($"\nFound {results.Count} movie(s):");
            foreach (var movie in results)
            {
                Console.WriteLine($"- {movie.Title} (Available: {movie.AvailableTickets}, Booked: {movie.BookedTickets})");
            }
        }

        static void AddMovie()
        {
            Console.Write("Enter movie title: ");
            string title = Console.ReadLine();

            Console.Write("Enter total number of seats: ");
            if (!int.TryParse(Console.ReadLine(), out int totalSeats) || totalSeats <= 0)
            {
                Console.WriteLine("Invalid number of seats. Please enter a positive number.");
                return;
            }

            if (bookingService.AddMovie(title, totalSeats))
            {
                Console.WriteLine($"Movie '{title}' added successfully.");
            }
            else
            {
                Console.WriteLine("Failed to add movie. Movie may already exist or invalid data provided.");
            }
        }

        static void DeleteMovie()
        {
            DisplayMovies();
            Console.Write("Enter movie number to delete: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= bookingService.GetMovies().Length)
            {
                int movieId = choice - 1; // Convert to zero-based index
                string movieTitle = bookingService.GetMovies()[movieId];

                Console.Write($"Are you sure you want to delete '{movieTitle}'? (Y/N): ");
                if (Console.ReadLine().Trim().ToUpper() == "Y")
                {
                    if (bookingService.DeleteMovie(movieId))
                    {
                        Console.WriteLine("Movie deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to delete movie.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid movie selection.");
            }
        }
    }
}
using System.Runtime.CompilerServices;
using System.Transactions;
using System;
using TicketBooking_BusinessDataLogic;

namespace TicketBooking
{
    internal class Program
    {
        static string[] userActions = { "[1] View Available Tickets", "[2] Book a Ticket", "[3] Cancel a Ticket",
                                      "[4] Select Another Movie", "[5] Search Movies", "[6] Exit" };

        static string[] adminActions = { "[1] View Available Tickets", "[2] Book a Ticket", "[3] Cancel a Ticket",
                                       "[4] Select Another Movie", "[5] Search Movies", "[6] Add Movie",
                                       "[7] Delete Movie", "[8] Add User", "[9] Exit" };

        static BookingService bookingService = new BookingService();
        static UserService userService = new UserService();
        static bool isAdmin = false;
        static string currentUsername = "";

        static void Main(string[] args)
        {
            Console.WriteLine("MOVIE TICKET BOOKING SYSTEM");

            // User authentication
            if (!Login())
            {
                Console.WriteLine("Login failed. Exiting system.");
                return;
            }

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

                    int exitOption = isAdmin ? 9 : 6;
                    if (userAction == exitOption)
                    {
                        Console.WriteLine("Exiting program...");
                        return;
                    }

                    HandleMovieAction(userAction, movieChoice);

                } while (userAction != 4 && userAction != (isAdmin ? 9 : 6));

            } while (movieChoice != -1);
        }

        static bool Login()
        {
            int attempts = 0;
            const int maxAttempts = 3;

            while (attempts < maxAttempts)
            {
                Console.Write("Username: ");
                string username = Console.ReadLine();

                Console.Write("Password: ");
                string password = ReadPassword();

                if (userService.AuthenticateUser(username, password, out bool userIsAdmin))
                {
                    isAdmin = userIsAdmin;
                    currentUsername = username;
                    Console.WriteLine($"\nWelcome {username}{(isAdmin ? " (Admin)" : "")}!");
                    return true;
                }

                attempts++;
                Console.WriteLine($"\nInvalid username or password. Attempts remaining: {maxAttempts - attempts}");
            }

            return false;
        }

        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Ignore any key that isn't alphanumeric or special character
                if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
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
                Console.Write("Select a Movie (0 to exit): ");
                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    if (choice == 0)
                    {
                        return -1;
                    }

                    string[] movies = bookingService.GetMovies();
                    if (choice >= 1 && choice <= movies.Length)
                    {
                        return choice - 1; 
                    }
                }
                Console.WriteLine("Invalid selection. Please try again.");

            } while (true);
        }

        static void DisplayActions()
        {
            Console.WriteLine("\n---------------------------------------");
            Console.WriteLine("\nACTIONS:");

            string[] actions = isAdmin ? adminActions : userActions;
            foreach (var action in actions)
            {
                Console.WriteLine(action);
            }
        }

        static int GetUserInput()
        {
            Console.Write("Select Action: ");
            if (int.TryParse(Console.ReadLine(), out int input))
            {
                int maxOption = isAdmin ? 9 : 6;
                if (input >= 1 && input <= maxOption)
                {
                    return input;
                }
                Console.WriteLine($"Invalid input. Please enter a number between 1 and {maxOption}.");
            }
            else
            {
                Console.WriteLine("Please enter a valid number.");
            }
            return GetUserInput();
        }

        static void HandleMovieAction(int action, int movieChoice)
        {
            // Actions common to both user and admin
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
            }

            // Admin-only actions
            if (isAdmin)
            {
                switch (action)
                {
                    case 6:
                        AddMovie();
                        break;
                    case 7:
                        DeleteMovie();
                        break;
                    case 8:
                        AddUser();
                        break;
                    case 9:
                        Console.WriteLine("Exiting...");
                        break;
                }
            }
            else if (action == 6)
            {
                Console.WriteLine("Exiting...");
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

        // Search functionality for all users
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

        // Admin-only methods
        static void AddMovie()
        {
            if (!isAdmin)
            {
                Console.WriteLine("You don't have permission to add movies.");
                return;
            }

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
            if (!isAdmin)
            {
                Console.WriteLine("You don't have permission to delete movies.");
                return;
            }

            DisplayMovies();
            Console.Write("Enter movie number to delete: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= bookingService.GetMovies().Length)
            {
                int movieId = choice - 1;
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

        static void AddUser()
        {
            if (!isAdmin)
            {
                Console.WriteLine("You don't have permission to add users.");
                return;
            }

            Console.Write("Enter new username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password for new user: ");
            string password = ReadPassword();

            Console.Write("Should this user be an admin? (Y/N): ");
            bool newUserIsAdmin = Console.ReadLine().Trim().ToUpper() == "Y";

            if (userService.RegisterUser(username, password, newUserIsAdmin))
            {
                Console.WriteLine($"User '{username}' added successfully as {(newUserIsAdmin ? "admin" : "regular user")}.");
            }
            else
            {
                Console.WriteLine("Failed to add user. Username may already exist.");
            }
        }
    }
}
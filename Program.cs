using System;
using BusinessDataLogicTicketBooking;

namespace Ticketbooking
{
    internal class Program
    {
        static string[] actions = { "[1] View Available Tickets", "[2] Book a Ticket", "[3] Cancel a Ticket", "[4] Select Another Movie", "[5] Exit" };
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

                    if (userAction == 5)
                    {
                        Console.WriteLine("Exiting program...");
                        return;
                    }

                    HandleMovieAction(userAction, movieChoice);

                } while (userAction != 4);

            } while (movieChoice != -1);
        }
        static void DisplayMovies()
        {
            Console.WriteLine("\nAvailable Movies:");
            string[] movies = BookingProcess.GetMovies();
            int[] availableTickets = BookingProcess.GetAvailableTickets();

            for (int i = 0; i < movies.Length; i++)
            {
                Console.WriteLine($"[{i + 1}] {movies[i]} - Available Tickets: {availableTickets[i]}");
            }
            Console.WriteLine("[5] Exit");
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
                    if (choice == 5)
                    {
                        return -1;
                    }

                    string[] movies = BookingProcess.GetMovies();
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
            foreach (var action in actions)
            {
                Console.WriteLine(action);
            }
        }
        static int GetUserInput()
        {
            Console.Write("Select Action: ");
            if (int.TryParse(Console.ReadLine(), out int input) && input >= 1 && input <= 5)
            {
                return input;
            }
            Console.WriteLine("Invalid input. Please enter a number between 1 and 5.");
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
                    Console.WriteLine("Exiting...");
                    break;
            }
        }
        static void DisplayAvailableTickets(int movieChoice)
        {
            string[] movies = BookingProcess.GetMovies();
            Console.WriteLine($"Available tickets for {movies[movieChoice]}: {BookingProcess.GetAvailableTicketsForMovie(movieChoice)}");
        }
        static void BookTicket(int movieChoice)
        {
            int availableTickets = BookingProcess.GetAvailableTicketsForMovie(movieChoice);

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

                if (BookingProcess.CheckTicketAvailability(movieChoice, numTickets))
                {
                    if (BookingProcess.UpdateTickets(Actions.BookTicket, movieChoice, numTickets))
                    {
                        Console.WriteLine("Ticket booked successfully.");
                    }
                }
                else
                {
                    Console.WriteLine("Unable to book tickets.");
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
            int bookedTickets = BookingProcess.GetBookedTicketsForMovie(movieChoice);

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

                if (BookingProcess.UpdateTickets(Actions.CancelTicket, movieChoice, numTickets))
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
    }
}
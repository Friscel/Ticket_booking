using TicketBookingCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBooking_BusinessDataLogic;
using TicketBookingDataService;

namespace TicketBookingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        MovieService movieService = new MovieService();

        [HttpGet]
        public IActionResult GetMovies()
        {
            var movieTitles = movieService.GetMovies();
            var availableTickets = movieService.GetAvailableTickets();

            var movies = movieTitles.Select((title, index) => new Movie
            {
                Title = title,
                AvailableTickets = availableTickets[index],
                BookedTickets = movieService.GetBookedTicketsForMovie(index)
            }).ToArray();

            return Ok(movies);
        }

        [HttpGet("search")]
        public IActionResult SearchMovies([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required");
            }

            var movies = movieService.SearchMovies(searchTerm);
            return Ok(movies);
        }

        [HttpGet("{movieIndex}/available-tickets")]
        public IActionResult GetAvailableTickets(int movieIndex)
        {
            var tickets = movieService.GetAvailableTicketsForMovie(movieIndex);
            if (tickets == -1)
            {
                return NotFound("Movie not found");
            }
            return Ok(tickets);
        }

        [HttpGet("{movieIndex}/booked-tickets")]
        public IActionResult GetBookedTickets(int movieIndex)
        {
            var tickets = movieService.GetBookedTicketsForMovie(movieIndex);
            if (tickets == -1)
            {
                return NotFound("Movie not found");
            }
            return Ok(tickets);
        }

        [HttpPatch("book-ticket")]
        public IActionResult BookTicket([FromBody] BookTicketRequest request)
        {
            if (request == null || request.MovieChoice < 0 || request.Amount <= 0)
            {
                return BadRequest("Invalid request data");
            }

            var result = movieService.UpdateTickets(Actions.BookTicket, request.MovieChoice, request.Amount);
            if (result)
            {
                return Ok(new { success = true, message = "Ticket booked successfully" });
            }
            return BadRequest(new { success = false, message = "Failed to book ticket" });
        }

        [HttpPatch("cancel-ticket")]
        public IActionResult CancelTicket([FromBody] CancelTicketRequest request)
        {
            if (request == null || request.MovieChoice < 0 || request.Amount <= 0)
            {
                return BadRequest("Invalid request data");
            }

            var result = movieService.UpdateTickets(Actions.CancelTicket, request.MovieChoice, request.Amount);
            if (result)
            {
                return Ok(new { success = true, message = "Ticket cancelled successfully" });
            }
            return BadRequest(new { success = false, message = "Failed to cancel ticket" });
        }

        [HttpPost]
        public IActionResult AddMovie([FromBody] AddMovieRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Title) || request.AvailableTickets <= 0)
            {
                return BadRequest("Invalid movie data");
            }

            var result = movieService.AddMovie(request.Title, request.AvailableTickets);
            if (result)
            {
                return Ok(new { success = true, message = "Movie added successfully" });
            }
            return BadRequest(new { success = false, message = "Failed to add movie. Movie may already exist." });
        }

        [HttpDelete("{movieIndex}")]
        public IActionResult DeleteMovie(int movieIndex)
        {
            if (movieIndex < 0)
            {
                return BadRequest("Invalid movie index");
            }

            var result = movieService.DeleteMovie(movieIndex);
            if (result)
            {
                return Ok(new { success = true, message = "Movie deleted successfully" });
            }
            return BadRequest(new { success = false, message = "Failed to delete movie" });
        }

        [HttpGet("check-availability")]
        public IActionResult CheckTicketAvailability([FromQuery] int movieChoice, [FromQuery] int amount)
        {
            if (movieChoice < 0 || amount <= 0)
            {
                return BadRequest("Invalid parameters");
            }

            var isAvailable = movieService.CheckTicketAvailability(movieChoice, amount);
            return Ok(new { available = isAvailable });
        }
    }

    // Request DTOs
    public class BookTicketRequest
    {
        public int MovieChoice { get; set; }
        public int Amount { get; set; }
    }

    public class CancelTicketRequest
    {
        public int MovieChoice { get; set; }
        public int Amount { get; set; }
    }

    public class AddMovieRequest
    {
        public string Title { get; set; }
        public int AvailableTickets { get; set; }
    }
}
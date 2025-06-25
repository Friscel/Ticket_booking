using TicketBookingCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketBooking_BusinessDataLogic;
using TicketBookingDataService;

namespace TicketBookingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        UserService userService = new UserService();

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { success = false, message = "Username and password are required" });
            }

            bool isAuthenticated = userService.AuthenticateUser(request.Username, request.Password, out bool isAdmin);

            if (isAuthenticated)
            {
                return Ok(new { success = true, isAdmin = isAdmin, message = "Login successful" });
            }

            return Unauthorized(new { success = false, message = "Invalid username or password" });
        }

        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] RegisterRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { success = false, message = "Username and password are required" });
            }

            var result = userService.RegisterUser(request.Username, request.Password, request.IsAdmin);

            if (result)
            {
                return Ok(new { success = true, message = "User registered successfully" });
            }

            return BadRequest(new { success = false, message = "Failed to register user. Username may already exist." });
        }
    }

    // Request DTOs
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
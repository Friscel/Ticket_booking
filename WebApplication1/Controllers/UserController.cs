using TicketBookingCommon;
using Microsoft.AspNetCore.Mvc;
using TicketBooking_BusinessDataLogic;
using TicketBookingDataService;
using TicketBookingWebAPI.Services; // ✅ Add this using statement

namespace TicketBookingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly EmailService _emailService;

        // ✅ Constructor with dependency injection for EmailService
        public UserController(EmailService emailService)
        {
            _userService = new UserService();
            _emailService = emailService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { success = false, message = "Username and password are required" });
            }

            bool isAuthenticated = _userService.AuthenticateUser(request.Username, request.Password, out bool isAdmin);

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

            var result = _userService.RegisterUser(request.Username, request.Password, request.IsAdmin);

            if (result)
            {
                // ✅ Send Mailtrap email notification after successful registration
                _emailService.SendEmail(
                    "test@yourmailtrap.io", // can be any email, Mailtrap captures it
                    "Registration Successful",
                    $"<h3>Welcome, {request.Username}!</h3><p>Your account has been successfully registered.</p>"
                );

                return Ok(new { success = true, message = "User registered successfully. Email sent." });
            }

            return BadRequest(new { success = false, message = "Failed to register user. Username may already exist." });
        }
    }

    // ✅ DTOs remain the same
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
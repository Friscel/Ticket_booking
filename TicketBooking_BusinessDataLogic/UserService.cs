using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingCommon;
using TicketBookingDataService;

namespace TicketBooking_BusinessDataLogic
{
    public class UserService
    {
        private UserDataService userDataService = new UserDataService();

        public bool AuthenticateUser(string username, string password, out bool isAdmin)
        {
            isAdmin = false;
            var users = userDataService.GetAllUsers();
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                isAdmin = user.IsAdmin;
                return true;
            }
            return false;
        }

        public bool RegisterUser(string username, string password, bool isAdmin)
        {
            var users = userDataService.GetAllUsers();
            if (users.Any(u => u.Username == username))
                return false;

            var newUser = new User
            {
                Username = username,
                Password = password,
                IsAdmin = isAdmin
            };

            userDataService.AddUser(newUser);
            return true;
        }
    }
}
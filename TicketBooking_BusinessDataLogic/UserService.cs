using TicketBookingDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketBooking_BusinessDataLogic
{
    public class UserService
    {
        private UserDataService userDataService = new UserDataService();

        public bool AuthenticateUser(string username, string password, out bool isAdmin)
        {
            return userDataService.ValidateUser(username, password, out isAdmin);
        }

        public bool RegisterUser(string username, string password, bool isAdmin)
        {
            return userDataService.AddUser(username, password, isAdmin);
        }
    }
}
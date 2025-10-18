using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingCommon;

namespace TicketBookingDataService
{
    public class UserDataService
    {
        IUserDataService userDataService;

        public UserDataService()
        {
            //userDataService = new InMemoryUserDataService();
            //userDataService = new TextFileUserDataService();
            //userDataService = new JsonFileUserDataService();
            userDataService = new DBUserDataService();
        }

        public List<User> GetAllUsers()
        {
            return userDataService.GetUsers();
        }

        public void AddUser(User user)
        {
            userDataService.CreateUser(user);
        }

        public void UpdateUser(User user)
        {
            userDataService.UpdateUser(user);
        }

        public void RemoveUser(User user)
        {
            userDataService.RemoveUser(user);
        }
    }
}
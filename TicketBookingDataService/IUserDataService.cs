using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingCommon;

namespace TicketBookingDataService
{
    public interface IUserDataService
    {
        List<User> GetUsers();
        void CreateUser(User user);
        void UpdateUser(User user);
        void RemoveUser(User user);
    }
}
using TicketBookingCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketBookingDataService
{
    public class UserDataService
    {
        private List<User> users = new List<User>();

        public UserDataService()
        {
            // Initialize with some users
            CreateDefaultUsers();
        }

        private void CreateDefaultUsers()
        {
            // Add an admin user
            users.Add(new User
            {
                Username = "admin",
                Password = "admin123",
                IsAdmin = true
            });

            // Add regular users
            users.Add(new User
            {
                Username = "user1",
                Password = "pass123",
                IsAdmin = false
            });

            users.Add(new User
            {
                Username = "user2",
                Password = "pass456",
                IsAdmin = false
            });
        }

        public bool ValidateUser(string username, string password, out bool isAdmin)
        {
            isAdmin = false;
            foreach (var user in users)
            {
                if (user.Username == username && user.Password == password)
                {
                    isAdmin = user.IsAdmin;
                    return true;
                }
            }
            return false;
        }

        public bool AddUser(string username, string password, bool isAdmin)
        {
            foreach (var user in users)
            {
                if (user.Username == username)
                    return false;
            }

            users.Add(new User
            {
                Username = username,
                Password = password,
                IsAdmin = isAdmin
            });
            return true;
        }
    }
}
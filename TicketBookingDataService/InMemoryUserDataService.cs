using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingCommon;

namespace TicketBookingDataService
{
    public class InMemoryUserDataService : IUserDataService
    {
        private static List<User> users = new List<User>();

        public InMemoryUserDataService()
        {
            if (users.Count == 0)
            {
                CreateDefaultUsers();
            }
        }

        private void CreateDefaultUsers()
        {
            users.Add(new User
            {
                Username = "admin",
                Password = "admin123",
                IsAdmin = true
            });

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

        public List<User> GetUsers()
        {
            return users;
        }

        public void CreateUser(User user)
        {
            users.Add(user);
        }

        public void UpdateUser(User user)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Username == user.Username)
                {
                    users[i].Password = user.Password;
                    users[i].IsAdmin = user.IsAdmin;
                    break;
                }
            }
        }

        public void RemoveUser(User user)
        {
            var userToRemove = users.FirstOrDefault(u => u.Username == user.Username);
            if (userToRemove != null)
            {
                users.Remove(userToRemove);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketBookingCommon;

namespace TicketBookingDataService
{
    public class TextFileUserDataService : IUserDataService
    {
        private string filePath = "users.txt";
        private List<User> users = new List<User>();

        public TextFileUserDataService()
        {
            GetDataFromFile();
        }

        private void GetDataFromFile()
        {
            if (!File.Exists(filePath))
            {
                CreateDefaultUsersFile();
            }

            var lines = File.ReadAllLines(filePath);
            users.Clear();

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        users.Add(new User
                        {
                            Username = parts[0],
                            Password = parts[1],
                            IsAdmin = Convert.ToBoolean(parts[2])
                        });
                    }
                }
            }
        }

        private void CreateDefaultUsersFile()
        {
            var defaultUsers = new List<string>
            {
                "admin|admin123|True",
                "user1|pass123|False",
                "user2|pass456|False"
            };
            File.WriteAllLines(filePath, defaultUsers);
        }

        private void WriteDataToFile()
        {
            var lines = new string[users.Count];
            for (int i = 0; i < users.Count; i++)
            {
                lines[i] = $"{users[i].Username}|{users[i].Password}|{users[i].IsAdmin}";
            }
            File.WriteAllLines(filePath, lines);
        }

        private int FindUserIndex(string username)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Username == username)
                {
                    return i;
                }
            }
            return -1;
        }

        public List<User> GetUsers()
        {
            return users;
        }

        public void CreateUser(User user)
        {
            users.Add(user);
            WriteDataToFile();
        }

        public void UpdateUser(User user)
        {
            int index = FindUserIndex(user.Username);
            if (index != -1)
            {
                users[index].Password = user.Password;
                users[index].IsAdmin = user.IsAdmin;
                WriteDataToFile();
            }
        }

        public void RemoveUser(User user)
        {
            int index = FindUserIndex(user.Username);
            if (index != -1)
            {
                users.RemoveAt(index);
                WriteDataToFile();
            }
        }
    }
}
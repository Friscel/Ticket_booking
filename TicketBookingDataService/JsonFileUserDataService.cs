using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using TicketBookingCommon;

namespace TicketBookingDataService
{
    public class JsonFileUserDataService : IUserDataService
    {
        private static List<User> users = new List<User>();
        private static string jsonFilePath = "users.json";

        public JsonFileUserDataService()
        {
            ReadJsonDataFromFile();
        }

        private void ReadJsonDataFromFile()
        {
            if (!File.Exists(jsonFilePath))
            {
                CreateDefaultUsersFile();
            }

            string jsonText = File.ReadAllText(jsonFilePath);
            if (!string.IsNullOrWhiteSpace(jsonText))
            {
                users = JsonSerializer.Deserialize<List<User>>(jsonText,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
        }

        private void CreateDefaultUsersFile()
        {
            var defaultUsers = new List<User>
            {
                new User { Username = "admin", Password = "admin123", IsAdmin = true },
                new User { Username = "user1", Password = "pass123", IsAdmin = false },
                new User { Username = "user2", Password = "pass456", IsAdmin = false }
            };

            users = defaultUsers;
            WriteJsonDataToFile();
        }

        private void WriteJsonDataToFile()
        {
            string jsonString = JsonSerializer.Serialize(users, new JsonSerializerOptions
            { WriteIndented = true });
            File.WriteAllText(jsonFilePath, jsonString);
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
            WriteJsonDataToFile();
        }

        public void UpdateUser(User user)
        {
            int index = FindUserIndex(user.Username);
            if (index != -1)
            {
                users[index].Password = user.Password;
                users[index].IsAdmin = user.IsAdmin;
                WriteJsonDataToFile();
            }
        }

        public void RemoveUser(User user)
        {
            int index = FindUserIndex(user.Username);
            if (index != -1)
            {
                users.RemoveAt(index);
                WriteJsonDataToFile();
            }
        }
    }
}
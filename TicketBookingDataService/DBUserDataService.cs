using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using TicketBookingCommon;

namespace TicketBookingDataService
{
    public class DBUserDataService : IUserDataService
    {
        private static string connectionString =
            "Data Source=LAPTOP-HDTASQBE\\SQLEXPRESS;Initial Catalog=TicketBookingDB;Integrated Security=True;TrustServerCertificate=True;";

        private static SqlConnection sqlConnection;

        public DBUserDataService()
        {
            sqlConnection = new SqlConnection(connectionString);
        }

        public List<User> GetUsers()
        {
            string selectStatement = "SELECT Id, Username, Password, IsAdmin FROM Users";
            SqlCommand selectCommand = new SqlCommand(selectStatement, sqlConnection);

            sqlConnection.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();

            var users = new List<User>();
            while (reader.Read())
            {
                User user = new User
                {
                    Username = reader["Username"].ToString(),
                    Password = reader["Password"].ToString(),
                    IsAdmin = Convert.ToBoolean(reader["IsAdmin"])
                };
                users.Add(user);
            }

            sqlConnection.Close();
            return users;
        }

        public void CreateUser(User user)
        {
            var insertStatement = "INSERT INTO Users (Username, Password, IsAdmin) VALUES (@Username, @Password, @IsAdmin)";
            SqlCommand insertCommand = new SqlCommand(insertStatement, sqlConnection);

            insertCommand.Parameters.AddWithValue("@Username", user.Username);
            insertCommand.Parameters.AddWithValue("@Password", user.Password);
            insertCommand.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);

            sqlConnection.Open();
            insertCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        public void UpdateUser(User user)
        {
            var updateStatement = "UPDATE Users SET Password = @Password, IsAdmin = @IsAdmin WHERE Username = @Username";
            SqlCommand updateCommand = new SqlCommand(updateStatement, sqlConnection);

            updateCommand.Parameters.AddWithValue("@Password", user.Password);
            updateCommand.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
            updateCommand.Parameters.AddWithValue("@Username", user.Username);

            sqlConnection.Open();
            updateCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        public void RemoveUser(User user)
        {
            var deleteStatement = "DELETE FROM Users WHERE Username = @Username";
            SqlCommand deleteCommand = new SqlCommand(deleteStatement, sqlConnection);

            deleteCommand.Parameters.AddWithValue("@Username", user.Username);

            sqlConnection.Open();
            deleteCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
}
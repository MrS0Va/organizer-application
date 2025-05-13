using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace organizer_application
{
    public static class DataBaseConnect
    {
        private static readonly string connectionString = "Data Source=WIN-GCR09CENVB3\\SQLEXPRESS;Initial Catalog=TaskOrganizer;Integrated Security=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static bool ValidateUser(string username, string password)
        {
            string passwordHash = PasswdHelper.HashPassword(password);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                int count = (int)command.ExecuteScalar();
                return count > 0; // Если есть хотя бы одна запись, значит, пользователь найден
            }
        }
        public static void RegisterUser(string username, string password)
        {
            string passwordHash = PasswdHelper.HashPassword(password);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                command.ExecuteNonQuery();
            }
        }
    }
}

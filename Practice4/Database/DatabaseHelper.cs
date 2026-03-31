using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Практическая_работа_4_Солодовников_Кураев.Database
{
    public class DatabaseHelper
    {
        private string _connectionString;
            
        public DatabaseHelper()
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["Practice6Part3Entities"];

            if (connectionStringSettings != null)
            {
                string efConnectionString = connectionStringSettings.ConnectionString;
                _connectionString = ExtractSqlConnectionString(efConnectionString);
            }
            else
            {
                _connectionString = @"Data Source=VASYADELL-LPC;Initial Catalog=Practice6Part3;Integrated Security=True;TrustServerCertificate=True";
            }
        }

        /// <summary>
        /// Извлекает обычную строку подключения из строки Entity Framework
        /// </summary>
        private string ExtractSqlConnectionString(string efConnectionString)
        {
            string marker = "provider connection string=\"";
            int start = efConnectionString.IndexOf(marker);
            if (start == -1)
                return efConnectionString;

            start += marker.Length;
            int end = efConnectionString.IndexOf("\"", start);
            if (end == -1)
                return efConnectionString;

            string result = efConnectionString.Substring(start, end - start);
            result = result.Replace("\"\"", "\"");
            return result;
        }

        /// <summary>
        /// Проверка существования пользователя
        /// </summary>
        public bool UserExists(string login)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Login = @Login";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Login", login);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Проверка логина и пароля
        /// </summary>
        public bool ValidateUser(string login, string password)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Login = @Login AND PasswordHash = @Password AND IsActive = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@Password", password);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Добавление нового пользователя
        /// </summary>
        public bool AddUser(string login, string password, string email = "", string fullName = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO Users (Login, PasswordHash, Email, FullName, IsActive, CreatedAt)
                        VALUES (@Login, @Password, @Email, @FullName, 1, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Login", login);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Email", email ?? "");
                        cmd.Parameters.AddWithValue("@FullName", fullName ?? "");
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка добавления пользователя: {ex.Message}");
                return false;
            }
        }
    }
}
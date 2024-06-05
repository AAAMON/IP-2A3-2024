using System;
using MySql.Data.MySqlClient;

namespace Server
{
    public sealed class ConnectionDB : IDisposable
    {
        static readonly string server = "localhost";
        static readonly string username = "alex";
        static readonly string password = "alex123K!!";
        static readonly string database = "dune_game";
        static readonly int port = 3000;
        readonly string connectionString = $"Server={server};Port={port};Database={database};Uid={username};Pwd={password};";
        private MySqlConnection connection;

        public ConnectionDB()
        {
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                Console.WriteLine("Connection with database works!");
                CreateTableIfNotExists();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public MySqlConnection GetConnection()
        {
            return connection;
        }

        private void CreateTableIfNotExists()
        {
            string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS players (
                id INT AUTO_INCREMENT PRIMARY KEY,
                username VARCHAR(255) UNIQUE,
                email VARCHAR(255),
                password VARCHAR(255)
            );";

            using (MySqlCommand command = new MySqlCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void AddPlayer(string username, string email, string password)
        {
            string addPlayerQuery = "INSERT INTO players (username, email, password) VALUES (@username, @password, @email);";

            using (MySqlCommand command = new MySqlCommand(addPlayerQuery, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@email", email);
                command.ExecuteNonQuery();
            }

            Console.WriteLine($"Player '{username}' added successfully.");
        }

        public bool PlayerExists(string username, string password)
        {
            string checkPlayerQuery = "SELECT COUNT(*) FROM players WHERE username = @username AND password = @password;";

            using (MySqlCommand command = new MySqlCommand(checkPlayerQuery, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }
        public bool PlayerExistDB(string username, string email)
        {
                        string checkPlayerQuery = "SELECT COUNT(*) FROM players WHERE username = @username AND password = @password;";

            using (MySqlCommand command = new MySqlCommand(checkPlayerQuery, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
                Console.WriteLine("Connection closed!");
            }
        }
    }
}

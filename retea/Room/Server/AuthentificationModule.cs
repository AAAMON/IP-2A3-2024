using System;
using System.Security.Cryptography;
using System.Text;

namespace authentification
{
    public class AuthenticationModule
    {
        private readonly IDatabase _database;

        public AuthenticationModule(IDatabase database)
        {
            _database = database;
        }

        public bool CreateUser(string username, string password)
        {
            if (_database.DoesUserExist(username))
            {
                Console.WriteLine("User already exists.");
                return false;
            }

            string hashedPassword = HashPassword(password);
            _database.InsertUser(username, hashedPassword);
            Console.WriteLine("User created successfully.");
            return true;
        }

        public bool CheckCredentials(string username, string password)
        {
            if (!_database.DoesUserExist(username))
            {
                Console.WriteLine("User does not exist.");
                return false;
            }

            string hashedPassword = _database.GetHashedPassword(username);
            string hashedInputPassword = HashPassword(password);

            return hashedPassword == hashedInputPassword;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public bool AuthenticateUser(string username, string password)
        {
            if (_database.DoesUserExist(username) == false) return false;

            string storedPassword = _database.GetHashedPassword(username);
            return storedPassword == HashPassword(password);
        }
    }
}
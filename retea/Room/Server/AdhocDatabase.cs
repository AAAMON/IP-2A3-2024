using System;
using System.Collections.Generic;

namespace authentification
{
    public class AdhocDatabase : IDatabase
    {
        private readonly Dictionary<string, string> userDatabase = new Dictionary<string, string>();

        public bool DoesUserExist(string username)
        {
            return userDatabase.ContainsKey(username);
        }

        public string GetHashedPassword(string username)
        {
            if (userDatabase.ContainsKey(username))
            {
                return userDatabase[username];
            }
            else
            {
                throw new ArgumentException("User does not exist in the database.");
            }
        }

        public void InsertUser(string username, string hashedPassword)
        {
            if (!userDatabase.ContainsKey(username))
            {
                userDatabase.Add(username, hashedPassword);
                Console.WriteLine("User added to the database.");
            }
            else
            {
                throw new ArgumentException("User already exists in the database.");
            }
        }
    }
}

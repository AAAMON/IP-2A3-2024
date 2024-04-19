using System;

namespace authentification
{
    public interface IDatabase
    {
        bool DoesUserExist(string username);
        string GetHashedPassword(string username);
        void InsertUser(string username, string hashedPassword);
    }
}

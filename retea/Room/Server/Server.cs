using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using roomconnectivity;
using authentification;
using System.Threading;

class Program
{
    static AdhocDatabase database = new AdhocDatabase();
    static AuthenticationModule module = new AuthenticationModule(database);
    static List<User> connectedUsers = new List<User>();
    static RoomManager roomManager = new RoomManager();
    static void Main(string[] args)
    {
        TcpListener server = null;
        try
        {
            // Set the TCP IP address and port on which to listen
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 12345;

            // Start listening for incoming client requests
            server = new TcpListener(ipAddress, port);
            server.Start();

            Console.WriteLine("Server started. Waiting for connections...");

            // Enter the listening loop
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected!");

                // Start a new thread to handle the client connection
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
        finally
        {
            // Stop listening for new clients
            server.Stop();
        }
    }

    static string ProcessCommand(string command, ref User user)
    {
        string[] parts = command.Split(' ');
        if (parts.Length == 0)
        {
            return "Invalid command.";
        }

        string commandName = parts[0];
        switch (commandName)
        {
            case "InsertUser":
                if (parts.Length < 3)
                {
                    return "Usage: InsertUser <username> <hashedPassword>";
                }
                string username = parts[1];
                string hashedPassword = parts[2];
                var result = module.CreateUser(username, hashedPassword);
                return result ? "successfull" : "unsuccessfull";

            case "GetHashedPassword":
                if (parts.Length < 2)
                {
                    return "Usage: GetHashedPassword <username>";
                }
                string password;
                try
                {
                    password = database.GetHashedPassword(parts[1]);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    password = e.Message;
                }
                return "Hashed password: " + password;

            case "DoesUserExist":
                if (parts.Length < 2)
                {
                    return "Usage: DoesUserExist <username>";
                }
                bool userExists = database.DoesUserExist(parts[1]);
                return "User exists: " + userExists;

            case "authenticate":
                if (parts.Length < 3)
                {
                    return "Usage: authenticate <username> <password>";
                }
                bool authenticated = module.AuthenticateUser(parts[1], parts[2]);
                if (authenticated == true)
                {
                    connectedUsers.Add(new User(parts[1]));
                    user = new User(parts[1]);
                    user.SetAuth(true);
                }
                return authenticated ? "Authentication successful." : "Authentication failed.";

            case "CreateRoom":
                if (user == null) return "You must authenticate first!";
                if (user.GetAuth() == false) return " User is not authenticated!";
                if (user.GetStatus() == true) return " User is already part of a room!";
                if (parts.Length < 2)
                {
                    return "Usage: authenticate <CreateRoom>";
                }

                string ret = roomManager.CreateRoom(parts[1]) ? "Successful connection to room!" : "Connection to room failed!";
                roomManager.GetRoomByName(parts[1]).AddUser(user);
                return ret;

            default:
                return "Unknown command.";
        }
    }


    static void HandleClient(TcpClient client)
    {
        User user = null;
        try
        {
            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            // Buffer for reading data
            byte[] bytes = new byte[256];
            int bytesRead;

            // Read the incoming message
            while ((bytesRead = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Convert bytes to string
                string command = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                Console.WriteLine($"Received: {command}");

                // Process the command using AdhocDatabase
                string response = ProcessCommand(command, ref user);

                // Send the response back to the client
                byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
                Console.WriteLine($"Sent: {response}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
        finally
        {
            // Close the client connection
            client.Close();
        }
    }


}




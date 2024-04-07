using System;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Connect to the server
                TcpClient client = new TcpClient("127.0.0.1", 12345);
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    // Read user input from the keyboard
                    Console.Write("Enter command: ");
                    string command = Console.ReadLine();

                    // Exit loop if user enters "exit"
                    if (command.ToLower() == "exit")
                        break;

                    // Send the command to the server
                    SendMessage(stream, command);
                    PrintResponse(stream);
                }

                // Close the connection
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        static void SendMessage(NetworkStream stream, string message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
            Console.WriteLine($"Sent: {message}");
        }

        static void PrintResponse(NetworkStream stream)
        {
            byte[] bytes = new byte[256];
            int bytesRead = stream.Read(bytes, 0, bytes.Length);
            string response = Encoding.ASCII.GetString(bytes, 0, bytesRead);
            Console.WriteLine($"Received: {response}");
        }
    }
}

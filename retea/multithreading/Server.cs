using System;
using System.Net;
using System.Net.Sockets;
namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 1300;
            string ipAdress="127.0.0.1";
            Socket ServerListener=new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAdress), port);
            ServerListener.Bind(ep);
            ServerListener.Listen(100);
            Console.WriteLine("Server is listening....");
            Socket ClientSocket=default(Socket);
            int counter = 0;
            Program p = new Program();
            while (true)
            {
                counter++;
                ClientSocket=ServerListener.Accept();
                Console.WriteLine(counter+" client is connected");
                Thread UserThread = new Thread(new ThreadStart(() => p.User(ClientSocket)));
                UserThread.Start();

            }
        }

        public void User(Socket client)
        {
            while (true)
            {
                byte[] msg = new byte[1024];
                int size = client.Receive(msg);

                client.Send(msg, 0, size, SocketFlags.None);
                
            }
        }
    }
}


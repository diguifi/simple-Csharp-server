using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleClient
{
    class Program
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            Console.Title = "Client";
            LoopConnect();
            LoopSend();
            Console.ReadLine();
        }

        private static void LoopSend()
        {
            while (true)
            {
                Console.Write("Enter a message: ");
                string text = Console.ReadLine();
                byte[] buffer = Encoding.ASCII.GetBytes(text);
                _clientSocket.Send(buffer);

                byte[] receivedBuf = new byte[1024];
                int received = _clientSocket.Receive(receivedBuf);
                byte[] data = new byte[received];
                Array.Copy(receivedBuf, data, received);
                Console.WriteLine("Server: "+ Encoding.ASCII.GetString(data));
            }
        }

        private static void LoopConnect()
        {
            int attempts = 0;

            while (!_clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    _clientSocket.Connect(IPAddress.Loopback, 6755);
                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine("Connection attempts: " + attempts.ToString());
                }
            }

            Console.Clear();
            Console.WriteLine("Connected to server!");
        }
    }
}

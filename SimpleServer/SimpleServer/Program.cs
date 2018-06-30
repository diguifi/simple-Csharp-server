using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleServer
{
    class Program
    {
        // buffer de dados dos clientes
        private static byte[] _buffer = new byte[1024];
        // lista de clientes
        private static List<Socket> _clientSockets = new List<Socket>();
        // socket do server (tcp)
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            Console.Title = "Server";
            Console.WriteLine("Welcome to Diego's Server");

            SetupServer();
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            // bind do socket a interface de rede no port 6755
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 6755));
            // listener (o valor 5 não é o maximo de conexoes,
            // se refere a quantas conexões pendentes podem ser
            // enfileiradas, ta mais pra um "buffer"
            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            Console.WriteLine("Server ready!");
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            // para de receber clientes momentaneamente
            Socket socket = _serverSocket.EndAccept(AR);
            // adiciona o cliente a lista de clientes
            _clientSockets.Add(socket);
            Console.WriteLine("Client connected");
            // começa a receber dados desse cliente
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            // volta a aceitar clientes
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int received = socket.EndReceive(AR);
            byte[] dataBuf = new byte[received];
            Array.Copy(_buffer, dataBuf, received);

            string text = Encoding.ASCII.GetString(dataBuf);
            Console.WriteLine("Text received: " + text);

            string response = string.Empty;

            if (text.ToLower() == "farendar")
            {
                response = "You need to sharpen your axe first";
            }
            else
            {
                response = "Text received";
            }

            byte[] data = Encoding.ASCII.GetBytes(response);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);

            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
    }
}

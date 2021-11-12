using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ChessServer
{
    internal class Server : IDisposable
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        private static TcpListener _tcpListener { get; set; }

        public void Dispose()
        {
            _tcpListener.Server.Dispose();
        }

        public void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            Console.WriteLine("Strating server...");

            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptSocket(TCPConnectCallBack, null);

            Console.WriteLine($"Server started on port: {Port}");
        }

        private static void TCPConnectCallBack(IAsyncResult result)
        {
            TcpClient Client = _tcpListener.EndAcceptTcpClient(result);
            _tcpListener.BeginAcceptTcpClient(TCPConnectCallBack, null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using ChessLib;

namespace ChessServer
{
    internal class Client : IDisposable
    {
        public string Name { get; private set; }
        private TcpClient _tcpClient { get; set; }
        private BinaryReader _binaryReader { get; set; }
        private BinaryWriter _binaryWriter { get; set; }
        private Chess _chess { get; set; }

        public Client(TcpClient tcpClient)
        {
            _binaryReader = new BinaryReader(tcpClient.GetStream());
            _binaryWriter = new BinaryWriter(tcpClient.GetStream());
            Name = _binaryReader.ReadString();
        }

        public void Dispose()
        {
            _tcpClient?.Dispose();
            _binaryReader?.Dispose();
            _binaryWriter?.Dispose();
        }
    }
}

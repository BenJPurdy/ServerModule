using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography;
using System.Threading;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var tgtEndPoint = new IPEndPoint(IPAddress.Loopback, 9050);
            var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);

            while (true)
            {
                var msg = Console.ReadLine() ?? "";
                socket.SendTo(Encoding.ASCII.GetBytes(msg), tgtEndPoint);
}
    }
}

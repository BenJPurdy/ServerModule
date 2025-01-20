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
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            while (true)
            {
                //while (socket.Available > 0)
                {
                    byte[] buffer = new byte[1024];
                    var rmt = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
                    var msg = Console.ReadLine() ?? "";
                    socket.SendTo(Encoding.ASCII.GetBytes(msg), tgtEndPoint);
                    int len = socket.ReceiveFrom(buffer, SocketFlags.None, ref rmt);
                    Console.WriteLine("SND:" + rmt.ToString() + 
                        " | UTC:" + DateTime.UtcNow + 
                        " | MSG:" + Encoding.ASCII.GetString(buffer, 0, len));
                }
               

            }
        }
    }
}

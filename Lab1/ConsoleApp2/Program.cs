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
        enum MessageType
        {
            Disconnect,
            Message,

        }
        static void Main(string[] args)
        {
            
            

            var tgtEndPoint = new IPEndPoint(IPAddress.Loopback, 9050);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ReceiveTimeout = 1000;
            socket.SendTimeout = 1000;



            byte[] buffer = new byte[1024];
            byte[] outBuffer = new byte[1024];
            int len = 0;
            var rmt = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                //while (socket.Available > 0)
                {
                    if (len > 0)
                    {
                        Console.WriteLine("SND:" + rmt.ToString() +
                            " | UTC:" + DateTime.UtcNow +
                            " | MSG:" + Encoding.ASCII.GetString(buffer, 0, len));
                    }
                    
                    var msg = Console.ReadLine() ?? "";
                    MessageType type;
                    

                    //get quit input here
                    if (true)
                    {
                        type = MessageType.Message;
                    }
                    Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                    if (msg != "")
                    {
                        MemoryStream ms = new MemoryStream();
                        BinaryWriter bw = new BinaryWriter(ms);

                        bw.Write((byte)type);
                        bw.Write(msg);
                        outBuffer = Encoding.ASCII.GetBytes(msg);
                        Console.WriteLine(ms.ToString());
                        socket.SendTo(ms.ToArray(), tgtEndPoint);
                    }
                    
                    len = socket.ReceiveFrom(buffer, SocketFlags.None, ref rmt);

                }
            }
        }
    }
}

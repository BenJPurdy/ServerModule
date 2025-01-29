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
using Server;


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
            var socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
            //socket.ReceiveTimeout = 1000;
            //socket.SendTimeout = 1000;
            socket.Connect(tgtEndPoint);
            PacketType type;
            byte[] byteBuffer = new byte[1024];
            byte[] outBuffer = new byte[1024];
            int len = 0;
            var rmt = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

            ConnectPacket init = new ConnectPacket();
            
            byte[] connectData;
            init.Serialise(out connectData);
            socket.SendTo(connectData, tgtEndPoint);

            len = socket.ReceiveFrom(byteBuffer, SocketFlags.None, ref rmt);
            bool run = true;
            while (run)
            {
                //while (socket.Available > 0)
                {
                    Packet p = Packet.Deserialise(byteBuffer);
                    
                    if (len > 0)
                    {
                        if (p is DataPacket d)
                        Console.WriteLine("SND:" + rmt.ToString() +
                            " | UTC:" + DateTime.UtcNow +
                            " | MSG:" + Encoding.ASCII.GetString(d.data, 0, (int)d.length));
                    }
                    
                    var msg = Console.ReadLine() ?? "";
                    //MessageType type;

                    Packet sendPacket;
                    //get quit input here
                    if (!msg.Equals("quit"))
                    {
                        sendPacket = new DataPacket
                        {
                            data = Encoding.ASCII.GetBytes(msg),
                        };
                        
                       
                    }
                    else
                    {
                        sendPacket = new DisconnectPacket { };
                    }
                    Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                    if (msg != "")
                    {
                        MemoryStream ms2 = new MemoryStream();
                        BinaryWriter bw = new BinaryWriter(ms2);

                        byte[] data;

                        /*
                        bw.Write((byte)type);
                        bw.Write(msg);
                        outBuffer = Encoding.ASCII.GetBytes(msg);
                        Console.WriteLine(ms.ToString());
                        socket.SendTo(ms.ToArray(), tgtEndPoint);
                        */
                        sendPacket.Serialise(out data);

                        socket.SendTo(data, rmt);

                        if (sendPacket is DisconnectPacket)
                        {
                            run = false;
                            break;
                        }

                    }
                    if (!run)
                    {
                        break;
                    }
                    len = socket.ReceiveFrom(byteBuffer, SocketFlags.None, ref rmt);

                }
            }

            DisconnectPacket end = new DisconnectPacket();

            byte[] endData;
            end.Serialise(out endData);
            socket.SendTo(endData, rmt);
        }
    }
}

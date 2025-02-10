using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    //connections are always FROM the server when dealing with local
    //or remote perspectives
    //
    internal class Connection
    {
        public Connection(IPEndPoint l, IPEndPoint r)
        {
            local = l;
            remote = r;
        }

        public void Receive(byte[] data)
        {
            Packet p = Packet.Deserialise(data);
            
            if (p is DataPacket d)
            {
                buffer = d.data;
            }
            
        }

        public void Send(Packet p, ref Socket s)
        {
            byte[] data;
            p.Serialise(out data);
            s.SendTo(data, remote);
        }

        

        public readonly IPEndPoint local;
        public readonly IPEndPoint remote;
        public UInt32 clientID;

        public byte[] buffer = new byte[1024];



    }
}

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
    internal class Connection
    {
        public Connection(IPEndPoint l, IPEndPoint r)
        {
            local = l;
            remote = r;
        }

        void Receive(byte[] data)
        {
            Packet p = Packet.Deserialise(data);
        }

        void Send(Packet p)
        {

        }

        

        public readonly IPEndPoint local;
        public readonly IPEndPoint remote;

        byte[] buffer = new byte[1024];



    }
}

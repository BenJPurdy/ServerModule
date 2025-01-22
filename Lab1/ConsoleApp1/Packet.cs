using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    enum PacketType
    {
        Connect,
        Disconnect,
        Data
    }

   

    internal class Packet
    {

        
        //the client wrt the packet is the client itself, not the packet reciever
        //client id can be assigned either by the client or by the server
        public uint client;

        //serialise (become bytes)
        public void Serialise(out byte[] outData)
        {

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(client);
            if (this is ConnectPacket) { bw.Write((byte)PacketType.Connect); }
            else if (this is DisconnectPacket) { bw.Write((byte)(PacketType.Disconnect)); }
            else if (this is DataPacket p) { 
                bw.Write((byte)PacketType.Data);
                bw.Write(p.data.Length);

            }

            outData = ms.ToArray();
            
        }

        //deserialise (become debytes)
        public static Packet Deserialise(byte[] indata)
        {

            MemoryStream ms = new MemoryStream(indata);
            BinaryReader bw = new BinaryReader(ms);
            var client = bw.ReadUInt32();
            byte type = bw.ReadByte();
            switch ((PacketType)type)
            {
                case PacketType.Connect:
                    return new ConnectPacket
                    {
                        client = client,
                    };

                case PacketType.Disconnect:
                    return new DisconnectPacket
                    {
                        client = client,
                    };
                case PacketType.Data:

                    return new DataPacket
                    {
                        client = client,


                    };

            }

            return null;
        }

    }

    internal class  ConnectPacket : Packet
    {
        
    }

    internal class DisconnectPacket : Packet
    {

    }

    internal class DataPacket : Packet
    {
        public byte[] data = new byte[1024];
    }


}

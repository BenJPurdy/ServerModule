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
        Data,

    }



    public class Packet
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
            else if (this is DataPacket p)
            {
                bw.Write((byte)PacketType.Data);
                bw.Write(p.data.Length);
                bw.Write(p.data);

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
                    UInt32 l = bw.ReadUInt32();
                    return new DataPacket
                    {
                        client = client,
                        length = l,
                        data = bw.ReadBytes((int)l)
                    };

            }

            return null;
        }

    }

    public class ConnectPacket : Packet
    {

    }

    public class DisconnectPacket : Packet
    {

    }

    public class DataPacket : Packet
    {
        public UInt32 length;
        public byte[] data = new byte[1024];
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using UnityEditor.PackageManager.Requests;


enum PacketType
    {
        Connect,
        Disconnect,
        Data,
        Transform,
        ReqestJoin,
        UniqueID,

    }



    public class Packet
    {

        //the client wrt the packet is the client itself, not the packet reciever
        //client id can be assigned either by the client or by the server
        public uint client;
        public uint length = sizeof(uint) + sizeof(byte);

    //serialise (become bytes)
    public void Serialise(out byte[] outData)
        {
            
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(client);
            if (this is ConnectPacket) { Debug.Log($"Connect Packet: "); bw.Write((byte)PacketType.Connect); }
            else if (this is DisconnectPacket) { Debug.Log($"Disconnect Packet: "); bw.Write((byte)(PacketType.Disconnect)); }
            else if (this is DataPacket p)
            {
            Debug.Log($"Data Packet: ");
 
                 bw.Write((byte)PacketType.Data);
                bw.Write(p.data.Length);
                bw.Write(p.data);
            }
            else if (this is RequestJoin)
            {
            Debug.Log($"Connect Packet: ");
            bw.Write((byte)PacketType.ReqestJoin);
            }
        else if (this is TransformPacket p1)
        {
            bw.Write((byte)PacketType.Transform);
            bw.Write(length + 32); //length
            bw.Write(p1.entity);
            bw.Write(p1.transformX);
            bw.Write(p1.transformY);
            bw.Write(p1.transformZ);

            bw.Write(p1.rotationX);
            bw.Write(p1.rotationY);
            bw.Write(p1.rotationZ);
            bw.Write(p1.padding);

        }

        outData = ms.ToArray();

        }

    //deserialise (become debytes)
    public static Packet Deserialise(byte[] indata)
    {
        //when the binary reader reads a value from the memorystream it moves to <T> bytes along the stream
        MemoryStream ms = new MemoryStream(indata);
        BinaryReader bw = new BinaryReader(ms);
        var client = bw.ReadUInt32();
        byte type = bw.ReadByte();
        uint l = bw.ReadUInt32();
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
                    length = l,
                    data = bw.ReadBytes((int)l)
                };
            case PacketType.Transform:
                uint e = bw.ReadUInt32();
                return new TransformPacket
                {
                    client = client,
                    entity = e,
                    transformX = bw.ReadSingle(),
                    transformY = bw.ReadSingle(),
                    transformZ = bw.ReadSingle(),
                    rotationX = bw.ReadSingle(),
                    rotationY = bw.ReadSingle(),
                    rotationZ = bw.ReadSingle(),
                    padding = bw.ReadUInt32()
                };
            case PacketType.UniqueID:

                uint id = bw.ReadUInt32();
                return new UniqueID
                {
                    client = client,
                    unique = id
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
        
        public byte[] data = new byte[1024];
    }

    public class TransformPacket : Packet
    {
        public TransformPacket() 
        { }
        public TransformPacket(uint id, Transform t)
        {
            entity = id;
            convertToPacketData(t.position, true);
            convertToPacketData(t.rotation.eulerAngles, false);
            length += 8 * sizeof(uint);
        }

    void convertToPacketData(Vector3 vec, bool isPosition)
    {
        if (isPosition)
        {
            transformX = vec.x; transformY = vec.y; transformZ = vec.z;
        }
        else
        {
            rotationX = vec.x; rotationY = vec.y; rotationZ = vec.z;
        }
    }

    public Vector3 getPos()
    {
        return new Vector3(transformX, transformY, transformZ);
    }

    public Quaternion getRot()
    {
        Quaternion quat = Quaternion.Euler(new Vector3(rotationX, rotationY, rotationZ));

        return quat;
    }
        public uint entity;
        public float transformX;
        public float transformY;
        public float transformZ;
    
        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public uint padding = 0;
    }

public class RequestJoin : Packet
{

}

public class UniqueID : Packet
{
    public uint unique;
}





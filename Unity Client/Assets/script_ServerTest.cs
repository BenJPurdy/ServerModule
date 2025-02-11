using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor.PackageManager;
using System;
using UnityEditor;
using System.Security.Cryptography;

public class NetworkManager : MonoBehaviour
{

    struct UdpState
    {
        UdpClient client;
        IPEndPoint endpoint;
    }

    byte[] receivedData;
    //bool badPacket;

    public static List<GameObject> networkObjects;
    
    static UdpState udpState;
    IPEndPoint endpoint;
    IPEndPoint remote;
    UdpClient udpClient = new UdpClient();
    // Start is called before the first frame update
    void Start()
    {
        endpoint = new IPEndPoint(IPAddress.Loopback, 9050);
        remote = new IPEndPoint(IPAddress.Any, 0);
        udpClient.Client.Blocking = true;
        udpClient.Connect(endpoint);
        Debug.Log("Client Complete");

        string msg = "UNITY CLIENT CONNECTING...";
        byte[] buff = Encoding.ASCII.GetBytes(msg);
        udpClient.Send(buff, buff.Length);

        foreach (var obj in networkObjects)
        {
            RequestJoin join = new RequestJoin();
            byte[] joinBytes;
            join.Serialise(out joinBytes);
            udpClient.Send(joinBytes, joinBytes.Length);
            bool recivedPacket = false;
            uint newID = 0;
            while (!recivedPacket)
            {
                byte[] packetData;
                packetData = udpClient.Receive(ref remote);
                Packet p = Packet.Deserialise(packetData);
                if (p is UniqueID p1)
                {
                    newID = p1.unique;
                    recivedPacket = true;
                }
            }
           obj.GetComponent<NetworkGameObject>().networkID = newID;
            

        }

        udpClient.Client.Blocking = false;
        //udpClient.BeginReceive(ReceiveAsyncCallback, udpState);
    
    }

    // Update is called once per frame
    void Update()
    {
        while (udpClient.Available > 0)
        {
            try
            {
                receivedData = udpClient.Receive(ref remote);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode != SocketError.WouldBlock)
                {
                    throw e;
                }
            }

            Packet p = Packet.Deserialise(receivedData);
            if (p == null)
            {
                continue;
            }
            if (p is TransformPacket)
            {

            }

        }
        

    }

    void SendPacket(Packet inPacket)
    {
        byte[] data = new byte[inPacket.length];
        inPacket.Serialise(out data);
        udpClient.Send(data, (int)inPacket.length);
        
    }

    void ReceiveAsyncCallback(IAsyncResult result)
    {
        byte[] iBuffer = udpClient.EndReceive(result, ref endpoint);

        udpClient.BeginReceive(ReceiveAsyncCallback, udpState);
    }

}

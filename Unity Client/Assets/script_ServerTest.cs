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

public class NetworkManager : MonoBehaviour
{

    struct UdpState
    {
        UdpClient client;
        IPEndPoint endpoint;
    }

    byte[] receivedData;
    //bool badPacket;
    
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

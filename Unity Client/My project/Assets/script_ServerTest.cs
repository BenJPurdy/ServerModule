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

public class script_ServerTest : MonoBehaviour
{

    struct UdpState
    {
        UdpClient client;
        IPEndPoint endpoint;
    }

    
    static UdpState udpState;
    IPEndPoint endpoint;
    UdpClient udpClient = new UdpClient();
    // Start is called before the first frame update
    void Start()
    {
        endpoint = new IPEndPoint(IPAddress.Loopback, 9050);
        udpClient.Connect(endpoint);
        Debug.Log("Client Complete");

        string msg = "UNITY CLIENT CONNECTING...";
        byte[] buff = Encoding.ASCII.GetBytes(msg);
        udpClient.Send(buff, buff.Length);

        udpClient.BeginReceive(ReceiveAsyncCallback, udpState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReceiveAsyncCallback(IAsyncResult result)
    {
        byte[] iBuffer = udpClient.EndReceive(result, ref endpoint);

        string iString = Encoding.ASCII.GetString(iBuffer);

        Debug.Log(iString);

        udpClient.BeginReceive(ReceiveAsyncCallback, udpState);
    }

}

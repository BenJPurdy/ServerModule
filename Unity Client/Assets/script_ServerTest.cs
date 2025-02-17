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
    
    public static List<GameObject> networkObjects = new();

    [SerializeField]
    GameObject[] gameObjects = new GameObject[10];
    
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
        udpClient.Client.ReceiveTimeout = 500;
        udpClient.Connect(endpoint);
        Debug.Log("Client Complete");
        DataPacket connectData = new DataPacket();
        string msg = "UNITY CLIENT CONNECTING...";
        connectData.data = Encoding.ASCII.GetBytes(msg);
        byte[] buff;
        connectData.Serialise(out buff);
        udpClient.Send(buff, buff.Length);

        
        foreach (var obj in networkObjects)
        {
            if (!obj.gameObject.GetComponent<NetworkGameObject>().isLocal) { continue; }
            RequestJoin join = new RequestJoin();
            byte[] joinBytes;
            join.Serialise(out joinBytes);
            //udpClient.Send(joinBytes, joinBytes.Length);
            bool recivedPacket = false;
            uint newID = 0;
            int attempts = 0;
            while (!recivedPacket)
            {
                udpClient.Send(joinBytes, joinBytes.Length);
                attempts++;
                if (attempts > 4) { Debug.Log("Did not recive a packet from the server");  break; }
                byte[] packetData;
                try
                {
                    packetData = udpClient.Receive(ref remote);
                    if (packetData == null)
                    {
                        Debug.Log("Packet was null");
                        continue;
                    }
                    Packet p = Packet.Deserialise(packetData);
                    if (p is UniqueID p1)
                    {
                        newID = p1.unique;
                        recivedPacket = true;
                    }
                }
                catch { Debug.Log($"Failed to recive data from the remote for {obj.name}, {attempts}"); }
            }
            obj.GetComponent<NetworkGameObject>().networkID = newID;
        }

        udpClient.Client.Blocking = false;
        //udpClient.BeginReceive(ReceiveAsyncCallback, udpState);
        for (int i = 0; i < networkObjects.Count; i++)
        {
            gameObjects[i] = networkObjects[i];
        }
    
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
            if (p is TransformPacket t)
            {
                Debug.Log($"Transform packet recived for {t.entity}");
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

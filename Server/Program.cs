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
using System.IO;
using System.Numerics;
using System.Diagnostics;



namespace Server
{

    public struct Vec3
    {
        public float x, y, z;

        public void Print()
        {
            Console.Write(x + ", " +  y + ", " + z + "\t");
        }

        void Read()
        {

        }
    }

    internal class Program
    {
        static void WriteLn(System.String iStr)
        {
            Console.WriteLine(iStr);
            Console.ReadLine();
        }

        //can use IPAddress.Parse("address_here") to create an IPAddress object for any address)
        //loopback goes to 127.0.0.1 (me, this machine, local address)
       

        static void Main(string[] args)
        {
            /*
             * socket 1
             *  -> data to network
             *  
             *  socket 2
             *      <- data from network
             *  
             *  socket 1 = socket 2
             */


            var localEndPoint = new IPEndPoint(IPAddress.Loopback, 9050);

            var localSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            localSocket.Blocking = true;
            localSocket.Bind(localEndPoint);

            Console.WriteLine("Socket Open");

            var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var REndPoint = (EndPoint)remoteEndPoint;

            string auth = "Server Auth :)";
            DataPacket authPacket = new DataPacket();
            authPacket.data = Encoding.ASCII.GetBytes(auth);
            authPacket.length = (uint)auth.Length;


            List<EndPoint> clients = new List<EndPoint>();
            List<IPEndPoint> ipClients = new List<IPEndPoint>();

            //connection/disconnection packets

            List<Connection> connections = new List<Connection>();
            uint UUID = 0;

            TransformPacket dummyPacket = new TransformPacket
            {
                entity = ++UUID,
                transformX = 1.2f,
                transformY = 2.0f,
                transformZ = 3.2f,

                rotationX = 1.2f,
                rotationY = 2.0f,
                rotationZ = 3.2f,
            };


            //server ingests data

            //determins who it's from

            //sends it to that connection for processing

            //send some auth back to that client

            while (true)
            {
                
                while (localSocket.Available > 0)
                {
                    byte[] recieveBuffer = new byte[1024];
                    var rmt = new IPEndPoint(IPAddress.Any, 0); //the endpoint data comes in to (refered to as the client)
                    EndPoint rmtEndPoint = rmt;
                    int len = localSocket.ReceiveFrom(recieveBuffer, ref rmtEndPoint);
                    rmt = (IPEndPoint)rmtEndPoint;
                    bool found = false;
                    int index = 0;

                    

                    for (int i = 0; i < connections.Count; i++)
                    {
                        Connection c = connections[i];
                        if (c == null)
                        {
                            index = i;
                            break;
                        }
                        if (rmt.Equals(c.remote))
                        {
                            found = true;
                            index = i;
                            break;
                        }
                        index = i+1;
                    }

                    if (!found)
                    {
                        if (connections.Count > index && connections[index] == null)
                        {
                            connections[index] = new Connection(localEndPoint, rmt);
                        }
                        else
                        {
                            connections.Add(new Connection(localEndPoint, rmt));
                        }
                       
                        
                        Console.WriteLine("New connection at " + index + ": " + connections[index].remote.ToString());
                        Console.WriteLine("MSG: ");
                        Console.WriteLine(Encoding.ASCII.GetString(recieveBuffer));

                       
                    }

                    Packet inPacket = Packet.Deserialise(recieveBuffer);
                    if (inPacket == null)
                    {
                        Console.WriteLine("Data in failed");
                    }
                    if (inPacket is DisconnectPacket)
                    {
                        for (int i = 0; i < connections.Count; i++)
                        {
                            {
                                Connection c = connections[i];
                                if (c == null) continue;
                                if (c.remote.Equals(rmt))
                                {
                                    Console.WriteLine("Removed " + c.remote.ToString() + " from connections");
                                    connections[i] = null;
                                }
                            }
                        }

                        //Console.WriteLine(authPacket.data);
                    }
                    if (inPacket is TransformPacket t)
                    {
                        Console.WriteLine($"{t.transformX}, {t.transformY}, {t.transformZ}, {t.rotationX}, {t.rotationY}, {t.rotationZ}");
                    }
                    if (inPacket is RequestID id)
                    {
                        Console.WriteLine("Data is request ID packet");
                        connections[index].Send(new UniqueID(++UUID), ref localSocket);
                    }
                    if (inPacket is DataPacket d)
                    {
                        Console.WriteLine(d.data);

                    }

                    // send the network objects to the clients
                    // list -> send all at once
                    // I have object, send object

                    if (connections[index] != null)
                    {
                        //loop over list of packets to send
                        
                        //send all packets
                        connections[index].Send(dummyPacket, ref localSocket);
                    }

                    found = false;

                }

                /*
                while (true)
                {
                    while(localSocket.Available > 0)
                    {
                        byte[] buffer = new byte[1024];
                        var rmt = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
                        int len = 0;

                        len = localSocket.ReceiveFrom(buffer, ref rmt);
                        if (!clients.Contains(rmt))
                        {
                            clients.Add(rmt);
                            string connectionMsg = "Connected to: " + localEndPoint.ToString();
                            Console.WriteLine("New Client: " + rmt.ToString() + " connected!");
                            try { localSocket.SendTo(Encoding.ASCII.GetBytes(connectionMsg), rmt); }
                            catch (Exception e) {Console.WriteLine(e.ToString());}
                        }

                        len = 0;
                        Console.WriteLine("Someone disconected");
                        clients.Remove(rmt);

                        foreach (var c in clients)
                        {
                            if (c != rmt)
                                localSocket.SendTo(buffer, c);
                        }
                        MemoryStream ms = new MemoryStream(buffer);
                        BinaryReader br = new BinaryReader(ms);
                        Console.WriteLine("| SENDER:" + rmt.ToString() 
                            + "\t| UTC:" + DateTime.UtcNow + 
                            "\n\t| TYPE:" + br.ReadByte() + 
                            "\t| MSG: " + br.ReadString());

                        Console.Write("USERLIST\n");
                        foreach (var c in clients)
                        {
                            Console.WriteLine("IP:" + c.ToString());
                            Console.WriteLine("-----------------------");
                        }
                    }

                }

                /*
                while socket available
                    get sender(any)
                    get length from sender
                    get messag from bytes
                    emit to console 



                */
            }
        }


        Connection findClient(List<Connection> connections, UInt32 client)
        {
            for (var i = 0; i < connections.Count; i++)
            {

            }
            return null;
        }
        
    }
}

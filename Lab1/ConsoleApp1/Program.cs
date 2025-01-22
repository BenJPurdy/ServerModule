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



namespace Server
{

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
           

            List<EndPoint> clients = new List<EndPoint>();
            List<IPEndPoint> ipClients = new List<IPEndPoint>();
            
            //connection/disconnection packets

            List<Connection> connections = new List<Connection>();

            //server ingests data

            //determins who it's from

            //sends it to that connection for processing

            //send some auth back to that client

            while(true)
            {
                {
                    byte[] buffer = new byte[1024];
                    var rmt = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint rmtEndPoint = rmt;
                    int len = localSocket.ReceiveFrom(buffer, ref rmtEndPoint);
                    rmt = (IPEndPoint)rmtEndPoint;
                    bool found = false;

                    for(int i = 0; i < connections.Count; i++) 
                    {
                        Connection c = connections[i];
                        if (rmt == c.remote)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        connections.Add(new Connection(localEndPoint, rmt));
                    }



                }
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
}

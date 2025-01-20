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



namespace ConsoleApp1
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
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Loopback, 9050);
            //var jackson = new IPEndPoint(IPAddress.Parse("194.66.32.10"), 9050);
            var jackson = new IPEndPoint(IPAddress.Parse("10.1.134.27"), 40005);
            


            int recv = 0;
            byte[] data = new byte[1024];
            string hi = "IM YELLING BACK";
            data = Encoding.ASCII.GetBytes(hi);
            TimeSpan tickTime = new TimeSpan(500);
            int tickCount = 0;
            long frameSep = 166667; //ticks = 100 ns 10,000,000 = 1t/s 1,000,000 = 10t/s 500,000 = 20t/s 166,667 = 60t/s
            long nextFrame = 0;

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(localEndPoint);
            //var dataLength = socket.ReceiveFrom(data, ref remote);

            //WriteLn("End");

            /*
             * socket 1
             *  -> data to network
             *  
             *  socket 2
             *      <- data from network
             *  
             *  socket 1 = socket 2
             */


            /*
            while (true)
            {

                long now = DateTime.Now.Ticks;
                if (now >= nextFrame)
                {
                    nextFrame = ((now / frameSep) + 1) * frameSep;

                    Console.Write("WE'RE SHOUTING INTO OUR OWN EAR\t | ");

                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    socket.SendTo(data, data.Length, SocketFlags.None, localEndPoint);
                    EndPoint remote = (EndPoint)remoteEndPoint;
                    recv = socket.ReceiveFrom(data, ref remote);
                    
                    //Server message
                    Console.Write("Message from " + remote.ToString() + "\t | ");
                    //data message back
                    Console.Write(Encoding.ASCII.GetString(data, 0, recv) + "\t | ");
                    //tracking
                    Console.Write(tickCount++ + "\r");

                    

                }

                
            }
            
            
            
            */


            int recv;
            byte[] buffer = new byte[1024];

            var localEndPoint = new IPEndPoint(IPAddress.Loopback, 9050);

            var localSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);

            localSocket.Bind(localEndPoint);

            Console.WriteLine("Socket Open");

            var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var REndPoint = (EndPoint)remoteEndPoint;

            recv = localSocket.ReceiveFrom(buffer, ref REndPoint);

            Console.WriteLine("Message recived from " + REndPoint.ToString() + " of size " + recv);
            Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, recv));
            string returnString = "Message recived and responded";
            buffer = Encoding.ASCII.GetBytes(returnString);

            localSocket.SendTo(buffer, buffer.Length, SocketFlags.None, REndPoint);
            Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, recv));


            
        }
    }
}

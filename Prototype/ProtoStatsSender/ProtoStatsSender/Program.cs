using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ProtoStatsSender
{
    class Sender
    {
        private volatile bool shouldStop = false;

        public int port = 11000;
        public int sleeptime = 0;
        public int maxmessages = 0;

        public void Stop()
        {
            shouldStop = true;
        }

        public void Start()
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress broadcast = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipendpoint = new IPEndPoint(broadcast, port);
            String message = "lorem|1";
            byte[] bytes = Encoding.ASCII.GetBytes(message.ToCharArray());
            s.Connect(ipendpoint);

            long i = 0;
            while (maxmessages <= 0 || i < maxmessages)
            {
                i++;
                Console.WriteLine(String.Format("Sending message {0}", i));
                s.Send(bytes);
                if ( sleeptime > 0 )
                    Thread.Sleep(sleeptime);
            }

        }
    }

    class Program
    {
        private const int port = 11000;

        static void Main(string[] args)
        {
            Sender sender = new Sender();
            sender.maxmessages = 150000;

            Thread t1 = new Thread(sender.Start);
            Thread t2 = new Thread(sender.Start);
            Thread t3 = new Thread(sender.Start);
            Thread t4 = new Thread(sender.Start);
            Thread t5 = new Thread(sender.Start);

            t1.Start();
         //   t2.Start();
           // t3.Start();
          //  t4.Start();
          //  t5.Start();

        }
    }
}

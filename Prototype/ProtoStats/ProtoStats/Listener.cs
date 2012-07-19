using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace ProtoStats
{
    class Listener
    {
        private volatile Object countersLock;
        private Dictionary<string, long> counters;
        private int port;

        private volatile bool stopListening = false;

        public long messageRcd = 0;
        public long badMessageCount = 0;

        public Listener(int port, Dictionary<string,long> counters, Object countersLock)
        {
            this.countersLock = countersLock;
            this.counters = counters;
            this.port = port;
        }

        public void Start()
        {
            long incValue = 0;

            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Any, port);
            UdpClient Listener = new UdpClient(port);
            Listener.Client.ReceiveTimeout = 1000;
            Listener.Client.ReceiveBufferSize = 32768;

            Console.WriteLine(String.Format("Listening on port {0}", port));

            while (!stopListening)
            {
                try
                {
                    // Receive UDP packet.
                    byte[] bytes = Listener.Receive(ref EndPoint);

                    messageRcd++;

                    // Decode.
                    // This could be nicer if it were broken out into a separate method etc, but I
                    // assume it's faster this way.  Don't know though.  Also the forceful inlining in .NET 4.5
                    // would probably help.
                    string message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    string[] parts = message.Split('|');

                    if (parts.Count() == 2 && long.TryParse(parts[1], out incValue))
                    {
                        UpdateCounter(parts[0], incValue);
                    }
                    else
                        badMessageCount++;
                }
                catch (SocketException)
                {
                }
            }

            Console.WriteLine("Listener stopping.");
        }


        public void Stop()
        {
            stopListening = true;
        }


        private void UpdateCounter(string counterName, long value)
        {
            lock (countersLock)
            {
                if (counters.ContainsKey(counterName))
                    counters[counterName] += value;
                else
                    counters.Add(counterName, value);
            }

            return;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProtoStats
{
    class Program
    {
        private const int port = 11000;

        private static Dictionary<String, long> counters = new Dictionary<string,long>();

        static void Main(string[] args)
        {
            Object countersLock = new Object();
            
            // Create a listening thread and start it.
            Listener listener = new Listener(port, counters, countersLock);
            Thread listenerThread = new Thread(new ThreadStart(listener.Start));
            listenerThread.Start();
        
            // Create the Saver thread and start it.
            Saver saver = new Saver(counters, countersLock);
            Thread saverThread = new Thread(new ThreadStart(saver.Start));
            saverThread.Start();

            // Wait for user to hit enter.
            Console.In.ReadLine();

            Console.WriteLine(String.Format("{0} rcd ({1} bad)", listener.messageRcd, listener.badMessageCount));
            Console.In.ReadLine();

            // Terminate threads.
            Console.WriteLine("Terminating Listener thread.");
            listener.Stop();
            listenerThread.Join(5000);
            if (listenerThread.IsAlive)
            {
                Console.WriteLine("Listener thread failed to terminate gracefully - aborting.");
                listenerThread.Abort();
            }

            Console.WriteLine("Terminating Saver thread.");
            saver.Stop();
            saverThread.Join(5000);
            if (saverThread.IsAlive)
            {
                Console.WriteLine("Saver thread failed to terminate gracefully - aborting.");
                saverThread.Abort();
            }
            
            Console.WriteLine("Done.");
            return;
        }
    }
}

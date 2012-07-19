using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProtoStats
{
    class Saver
    {
        private Dictionary<string, long> counters;
        private volatile Object countersLock;
        private volatile bool stopSaving = false;

        public Saver(Dictionary<string, long> counters, Object countersLock)
        {
            this.counters = counters;
            this.countersLock = countersLock;
        }

        public void Start()
        {
            Dictionary<string, long> countersCopy;

            Console.WriteLine("Getting ready to save...");

            while (!stopSaving)
            {
                Thread.Sleep(1000);

                lock (countersLock)
                {
                    countersCopy = new Dictionary<string, long>(counters);
                }

                RenderCounters(countersCopy);
            }

            Console.WriteLine("Saver terminating.");
        }

        public void Stop()
        {
            stopSaving = true;
        }

        private void RenderCounters(Dictionary<String, long> counters)
        {
            Console.WriteLine("Current state of counters:");
            foreach (string counter in counters.Keys)
            {
                Console.WriteLine(String.Format("\t{0}: {1}", counter, counters[counter]));
            }
            Console.WriteLine();
        }
    }
}

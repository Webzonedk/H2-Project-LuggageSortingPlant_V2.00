using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace LuggageSortingPlant_V2._00
{
    class LuggageController

    {
        public EventHandler luggageCreated;
        //   public LuggageWorker luggageWorker = new LuggageWorker();
        public Thread LuggageControllerThread { get; set; }

        public LuggageController()
        {
            Thread thread = new Thread(RunThread);
            thread.Start();
        }

        public void RunThread()
        {
            while (true)
            {

                int counter = FindLastFreeIndex();
                                           //Logi getting number of luggage
             luggageCreated?.Invoke(this, new LuggageEvent(counter));//Invoking the luggage and send it to the listener

            }
        }
        private int FindLastFreeIndex()
        {

          
                Monitor.Enter(MainServer.luggageBuffer);
            try
            {
                int temp = 0;
                for (int i = 0; i < MainServer.luggageBuffer.Length; i++)
                {
                    if (MainServer.luggageBuffer[i] != null)
                    {
                        temp++;
                    }
                }
                return temp;     

            }
            finally
            {
                Monitor.PulseAll(MainServer.luggageBuffer);
                Monitor.Exit(MainServer.luggageBuffer);
            }

        }
    }


}

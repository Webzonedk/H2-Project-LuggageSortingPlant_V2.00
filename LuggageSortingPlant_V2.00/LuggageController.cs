using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace LuggageSortingPlant_V2._00
{
    //--------------------------------------------------------------
    //This is a controller class to get the count of Luggage running around in the hall,
    //looking for a checkIn counter.
    //--------------------------------------------------------------
    class LuggageController

    {



        public EventHandler countLuggage;
        public Thread LuggageControllerThread { get; set; }

        public LuggageController()
        {
            Thread luggageBufferThread = new Thread(RunThread);
            luggageBufferThread.Start();
        }

        public void RunThread()
        {
            while (true)
            {
                int counter = CountLuggageInLuggageBuffer();
                countLuggage?.Invoke(this, new LuggageEvent(counter));//Invoking the luggage and send it to the listener
               Thread.Sleep(50);
            }
        }


        private int CountLuggageInLuggageBuffer()
        {
            // Monitor.Enter(MainServer.luggageBuffer);
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
                //Monitor.PulseAll(MainServer.luggageBuffer);
                //Monitor.Exit(MainServer.luggageBuffer);
            }

        }
    }


}

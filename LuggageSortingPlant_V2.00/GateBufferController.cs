using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class GateBufferController
    {
        public EventHandler countGateBufferLuggage;
      //  private Thread gateLuggageCounterThread;
        private int gateNumber;

        public int GateNumber
        {
            get { return gateNumber; }
            set { gateNumber = value; }
        }


        //public Thread GateLuggageCounterThread
        //{
        //    get { return gateLuggageCounterThread; }
        //    set { gateLuggageCounterThread = value; }
        //}



        public GateBufferController(int gateNumber)
        {
            this.gateNumber = gateNumber;
            Thread countLuggageInGateBuffer = new Thread(Run);
            countLuggageInGateBuffer.Start();
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    if (MainServer.gateBuffers[GateNumber] != null)
                    {
                        int count = CountLuggageInGateBuffer();
                        countGateBufferLuggage?.Invoke(this, new GateBufferEvent(GateNumber, count));//Invoking the luggage and send it to the listener
                    }
                    Thread.Sleep(50);
                }
                finally
                {
                }
            }
        }

        private int CountLuggageInGateBuffer()
        {
            try
            {
                //Monitor.Enter(MainServer.checkInBuffers[CheckInNumber]);
                int count = 0;
                for (int i = 0; i < MainServer.gateBuffers[GateNumber].Buffer.Length; i++)
                {
                    if (MainServer.gateBuffers[GateNumber].Buffer[i] != null)
                    {
                        count++;
                    }
                }
                return count;
            }
            finally
            {
                //Monitor.PulseAll(MainServer.checkInBuffers[CheckInNumber]);
                //Monitor.Exit(MainServer.checkInBuffers[CheckInNumber]);
            }

        }
    }
}

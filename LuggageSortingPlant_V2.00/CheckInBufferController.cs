using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class CheckInBufferController
    {
        public EventHandler countCheckInBufferLuggage;
        private Thread checkInLuggageCounterThread;
        private int checkInNumber;

        public int CheckInNumber
        {
            get { return checkInNumber; }
            set { checkInNumber = value; }
        }


        public Thread CheckInLuggageCounterThread
        {
            get { return checkInLuggageCounterThread; }
            set { checkInLuggageCounterThread = value; }
        }



        public CheckInBufferController(int checkInNumber)
        {
            this.checkInNumber = checkInNumber;
            Thread countLuggageInBuffer = new Thread(Run);
            countLuggageInBuffer.Start();
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    if (MainServer.checkInBuffers[CheckInNumber] != null)
                    {
                        int count = CountLuggageInLuggageBuffer();
                        countCheckInBufferLuggage?.Invoke(this, new CheckInBufferEvent(CheckInNumber, count));//Invoking the luggage and send it to the listener
                    }
                  Thread.Sleep(50);
                }
                finally
                {
                }
            }
        }

        private int CountLuggageInLuggageBuffer()
        {
            try
            {
                //Monitor.Enter(MainServer.checkInBuffers[CheckInNumber]);
                int count = 0;
                for (int i = 0; i < MainServer.checkInBuffers[CheckInNumber].Buffer.Length; i++)
                {
                    if (MainServer.checkInBuffers[CheckInNumber].Buffer[i] != null)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class CheckInBuffer
    {
        #region Fields
        private int checkInNumber;
        private Luggage[] buffer = new Luggage[MainServer.checkInBufferSize];
        #endregion



        #region Properties
        public int CheckInNumber
        {
            get { return checkInNumber; }
            set { checkInNumber = value; }
        }
        public Luggage[] Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }
        #endregion



        #region Constructors
        public CheckInBuffer()
        {

        }
        //initializing
        public CheckInBuffer(int checkInNumber)
        {
            this.checkInNumber = checkInNumber;
        }
        #endregion



        #region Methods
        public void ReorderingCheckInBuffer()//Reordering the checkin Buffers to work as a queue with first in first out.
        {
            while (true)
            {
                Monitor.Enter(MainServer.checkInBuffers[CheckInNumber]);//Locking the thread
                try
                {
                    for (int i = 0; i < MainServer.checkInBuffers[CheckInNumber].Buffer.Length - 1; i++)
                    {
                        if (MainServer.checkInBuffers[CheckInNumber].Buffer[i] == null)
                        {
                            MainServer.checkInBuffers[CheckInNumber].Buffer[i] = MainServer.checkInBuffers[CheckInNumber].Buffer[i + 1];
                            MainServer.checkInBuffers[CheckInNumber].Buffer[i + 1] = null;
                        }
                    }
                }
                finally
                {
                    Monitor.PulseAll(MainServer.checkInBuffers[CheckInNumber]);//Sending signal to other thread
                    Monitor.Exit(MainServer.checkInBuffers[CheckInNumber]);//Release the lock
                }
               // Thread.Sleep(MainServer.random.Next(MainServer.randomSleepMin, MainServer.randomSleepMax));
                Thread.Sleep(MainServer.basicSleep);
            }
        }
        #endregion
    }
}


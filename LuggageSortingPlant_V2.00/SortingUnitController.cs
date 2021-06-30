using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class SortingUnitController
    {

        public EventHandler countLuggageInSortingUnitBuffer;

        public SortingUnitController()
        {
            Thread sortingUnitBufferThread = new Thread(RunThread);
            sortingUnitBufferThread.Start();
        }

        public void RunThread()
        {
            while (true)
            {
                int counter = CountLuggageInSortingUnitBuffer();
                countLuggageInSortingUnitBuffer?.Invoke(this, new SortingUnitEvent(counter));//Invoking the luggage and send it to the listener
                Thread.Sleep(50);
            }
        }


        private int CountLuggageInSortingUnitBuffer()
        {
            try
            {
                int count = 0;
                for (int i = 0; i < MainServer.sortingUnitBuffer.Length; i++)
                {
                    if (MainServer.sortingUnitBuffer[i] != null)
                    {
                        count++;
                    }
                }
                return count;
            }
            finally
            {

            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class SortingUnitQueueWorker
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructors
        public SortingUnitQueueWorker()
        {

        }
        #endregion

        #region Methods
        public void ReorderingSortingUnitBuffer()//Reordering the checkin Buffers to work as a queue with first in first out.
        {
            while (true)
            {
                //if (MainServer.sortingUnitBuffer[MainServer.sortingUnitBuffer.Length - 1] !=null)
                //{

                    Monitor.Enter(MainServer.sortingUnitBuffer);//Locking the thread
                try
                {
                    for (int i = 0; i < MainServer.sortingUnitBuffer.Length - 1; i++)
                    {
                        if (MainServer.sortingUnitBuffer[i] == null)
                        {
                            MainServer.sortingUnitBuffer[i] = MainServer.sortingUnitBuffer[i + 1];
                            MainServer.sortingUnitBuffer[i + 1] = null;
                        }
                    }
                }
                finally
                {
                    Monitor.PulseAll(MainServer.sortingUnitBuffer);//Sending signal to other thread
                    Monitor.Exit(MainServer.sortingUnitBuffer);//Release the lock

                }
                //}
                Thread.Sleep(1);
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
//------------------------------------------------------------------------------------------------------------------
//This class is ment to reordering the luggage array to work as a queue
//------------------------------------------------------------------------------------------------------------------
namespace LuggageSortingPlant_V2._00
{
    class LuggageQueueWorker
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructors
        public LuggageQueueWorker()
        {

        }
        #endregion

        #region Methods

        #endregion
        public void ReorderingLuggageBuffer()//Reordering the LuggageBuffer to work as a queue with first in first out.
        {
            while (true)
            {

                //if (MainServer.luggageBuffer[MainServer.luggageBuffer.Length - 1] != null)
                //{
                    Monitor.Enter(MainServer.luggageBuffer);//Locking the thread
                    try
                    {
                        for (int i = 0; i < MainServer.luggageBuffer.Length - 1; i++)
                        {
                            if (MainServer.luggageBuffer[i] == null)
                            {
                                MainServer.luggageBuffer[i] = MainServer.luggageBuffer[i + 1];
                                MainServer.luggageBuffer[i + 1] = null;
                            }
                        }
                    }
                    finally
                    {
                        Monitor.PulseAll(MainServer.luggageBuffer);//Sending signal to other thread
                        Monitor.Exit(MainServer.luggageBuffer);//Release the lock

                    }
                //}
                Thread.Sleep(1);
            }
        }
    }
}

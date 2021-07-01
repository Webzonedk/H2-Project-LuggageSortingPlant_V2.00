using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class FlightPlanLogQueueWorker
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructors
        public FlightPlanLogQueueWorker()
        {

        }
        #endregion

        #region Methods
        public void ReorderingFlightPlanLog()//Reordering the flightplanbuffer to work as a queue with first in first out.
        {
            while (true)
            {
                Monitor.Enter(MainServer.flightPlanLog);//Locking the thread
                try
                {
                    for (int i = 0; i < MainServer.flightPlanLog.Length - 1; i++)
                    {
                        if (MainServer.flightPlanLog[i] == null)
                        {
                            MainServer.flightPlanLog[i] = MainServer.flightPlanLog[i + 1];
                            MainServer.flightPlanLog[i + 1] = null;
                        }
                        //else
                        //{
                        //    Monitor.Wait(MainServer.flightPlanLog);
                        //}
                    }

                    //Counting how many logs there are in the array
                    int counter = 0;
                    for (int i = 0; i < MainServer.flightPlanLog.Length; i++)
                    {
                        if (MainServer.flightPlanLog[i] != null)
                        {
                            counter++;
                        }
                    }
                    if (counter >= MainServer.logSize - 50)// If there is more than max logs - 50, then delete the 20 oldest logs
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            MainServer.flightPlanLog[i] = null;
                        }
                    }
                }
                finally
                {
                    Monitor.PulseAll(MainServer.flightPlanLog);//Sending signal to other thread
                    Monitor.Exit(MainServer.flightPlanLog);//Release the lock

                }
                Thread.Sleep(1);
            }
        }
        #endregion
    }
}

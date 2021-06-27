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
                try
                {

                    Monitor.Enter(MainServer.flightPlanLog);//Locking the thread

                    for (int i = 0; i < MainServer.flightPlanLog.Length - 1; i++)
                    {
                        if (MainServer.flightPlanLog[i] == null)
                        {
                            MainServer.flightPlanLog[i] = MainServer.flightPlanLog[i + 1];
                            MainServer.flightPlanLog[i + 1] = null;
                        }
                        else
                        {
                            Monitor.Wait(MainServer.flightPlanLog);
                        }
                    }
                    int counter = 0;
                    //Counting how many logs there are in the array
                    for (int i = 0; i < MainServer.flightPlanLog.Length; i++)
                    {
                        if (MainServer.flightPlanLog[i] != null)
                        {
                            counter++;
                        }
                    }
                    if (counter >= MainServer.logSize - 1000)// If there is more than 90 logs, then delete the 20 oldest logs
                    {
                        for (int i = 0; i < 2000; i++)
                        {
                            MainServer.flightPlanLog[0 + i] = null;
                        }
                    }
                }
                finally
                {
                    Monitor.PulseAll(MainServer.flightPlanLog);//Sending signal to other thread
                    Monitor.Exit(MainServer.flightPlanLog);//Release the lock

                }
                Thread.Sleep(50);
            }
        }
        #endregion
    }
}

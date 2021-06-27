using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class FlightPlanQueueWorker
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructors
        public FlightPlanQueueWorker()
        {

        }
        #endregion

        #region Methods
        public void ReorderingFlightPlan()//Reordering the flightplanbuffer to work as a queue with first in first out.
        {
            while (true)
            {
                if (MainServer.flightPlans[MainServer.maxPendingFlights-1] != null)
                {
                    try
                    {
                        Monitor.Enter(MainServer.flightPlans);//Locking the thread
                        for (int i = 0; i < MainServer.flightPlans.Length - 1; i++)
                        {
                            if (MainServer.flightPlans[i] == null)
                            {
                                MainServer.flightPlans[i] = MainServer.flightPlans[i + 1];
                                MainServer.flightPlans[i + 1] = null;
                            }
                        }
                    }
                    finally
                    {
                        Monitor.PulseAll(MainServer.flightPlans);//Sending signal to other thread
                        Monitor.Exit(MainServer.flightPlans);//Release the lock
                    }
                }
                Thread.Sleep(50);
            }
        }
        #endregion
    }
}

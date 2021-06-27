using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class FlightPlanWorker
    {
        #region Fields
        private string workerName;

        #endregion

        #region Properties

        public string WorkerName
        {
            get { return workerName; }
            set { workerName = value; }
        }

        #endregion

        #region Constructors
        public FlightPlanWorker()
        {

        }
        public FlightPlanWorker(string workerName)
        {
            this.workerName = workerName;
        }
        #endregion

        #region Methods

        #endregion
        //Adding flights if the flightbuffer is not full
        public void AddFlightToFlightPlan()
        {
            int flightNumberCounter = 0;
            while (true)
            {

                if (MainServer.flightPlans[MainServer.maxPendingFlights - 1] == null)
                {
                    int destinationIndex = MainServer.random.Next(0, MainServer.destinations.Length);
                    int seats = MainServer.random.Next(0, MainServer.numberOfSeats.Length);
                    FlightPlan flightPlan = new FlightPlan();
                    flightPlan.FlightNumber = flightNumberCounter;
                    flightNumberCounter++;
                    flightPlan.Destination = MainServer.destinations[destinationIndex];
                    flightPlan.Seats = MainServer.numberOfSeats[seats];
                    flightPlan.GateNumber = MainServer.random.Next(0, MainServer.amountOfGates);

                    if (MainServer.flightPlans[MainServer.maxPendingFlights - 2] == null)
                    {
                        flightPlan.DepartureTime = DateTime.Now.AddSeconds(MainServer.random.Next(MainServer.flightPlanMinInterval, MainServer.flightPlanMaxInterval));
                    }
                    else
                    {
                        flightPlan.DepartureTime = MainServer.flightPlans[MainServer.maxPendingFlights - 2].DepartureTime.AddSeconds(MainServer.random.Next(MainServer.flightPlanMinInterval, MainServer.flightPlanMaxInterval));
                    }
                    try
                    {
                        Monitor.Enter(MainServer.flightPlans);//Locking the thread
                        MainServer.flightPlans[MainServer.maxPendingFlights - 1] = flightPlan;
                        MainServer.outPut.PrintFlightPlan(MainServer.maxPendingFlights - 1);//Send parameter with the method
                    }
                    finally
                    {
                        Monitor.PulseAll(MainServer.flightPlans);//Sending signal to other thread
                        Monitor.Exit(MainServer.flightPlans);//Release the lock
                    }
                }
                Thread.Sleep(MainServer.random.Next(MainServer.randomSleepMin, MainServer.randomSleepMax));
            }
        }
    }
}

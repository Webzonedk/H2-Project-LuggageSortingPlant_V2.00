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
            FlightPlan[] tempFlightPlan = new FlightPlan[1];

            int flightNumberCounter = 0;
            while (true)
            {

                if (MainServer.flightPlans[MainServer.maxPendingFlights - 1] == null)
                {
                    FlightPlan tempFlightObject = new FlightPlan();
                    int destinationIndex = MainServer.random.Next(0, MainServer.destinations.Length);
                    int seats = MainServer.random.Next(0, MainServer.numberOfSeats.Length);
                    tempFlightObject.FlightNumber = flightNumberCounter;
                    flightNumberCounter++;
                    tempFlightObject.Destination = MainServer.destinations[destinationIndex];
                    tempFlightObject.Seats = MainServer.numberOfSeats[seats];
                    tempFlightObject.GateNumber = MainServer.random.Next(0, MainServer.amountOfGates);

                    if (MainServer.flightPlans[MainServer.maxPendingFlights - 2] == null)
                    {
                        tempFlightObject.DepartureTime = DateTime.Now.AddSeconds(MainServer.random.Next(MainServer.flightPlanMinInterval, MainServer.flightPlanMaxInterval));
                    }
                    else
                    {
                        tempFlightObject.DepartureTime = MainServer.flightPlans[MainServer.maxPendingFlights - 2].DepartureTime.AddSeconds(MainServer.random.Next(MainServer.flightPlanMinInterval, MainServer.flightPlanMaxInterval));
                    }
                    tempFlightPlan[0] = tempFlightObject;
                    Monitor.Enter(MainServer.flightPlans);//Locking the thread
                    try
                    {
                        Array.Copy(tempFlightPlan, 0, MainServer.flightPlans, MainServer.maxPendingFlights - 1, 1);//Copy first index from tempLuggage to the last index in the luggage buffer array
                        tempFlightPlan[0] = null;
                        //  MainServer.outPut.PrintFlightPlan(MainServer.maxPendingFlights - 1);//Send parameter with the method
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

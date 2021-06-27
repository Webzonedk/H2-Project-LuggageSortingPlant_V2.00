using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Bogus;
//-----------------------------------------------------------------------------------------
//This class is made to create luggage. It is a thread worker
//-----------------------------------------------------------------------------------------
namespace LuggageSortingPlant_V2._00
{
    class LuggageWorker
    {
        #region Fields

        #endregion
        public EventHandler luggageCreated;


        #region Properties

        #endregion


        //Initializing the class
        #region Constructors
        public LuggageWorker()
        {

        }

        #endregion



        #region Methods
        public void CreateLuggage()
        {
            int luggageCounter = 1;
            int paasengerNumber = 1;
            while (true)
            {


                //try
                //{


                if (MainServer.luggageBuffer[MainServer.MaxLuggageBuffer - 1] == null)
                {
                    //Added a check to ensure that the randomMax will not exceed the amount og flights in the flightplan
                    int randomMax = 0;
                    try
                    {
                        //   Monitor.Enter(MainServer.flightPlans);//Locking the flightPlan lock
                        for (int i = 0; i < MainServer.flightPlans.Length; i++)
                        {
                            if (MainServer.flightPlans[i] != null)
                            {
                                MainServer.tempFlightPlans[randomMax] = MainServer.flightPlans[i];//Adding the flightplans from the array to a temporary array
                                randomMax++;
                            }
                        }
                    }
                    finally
                    {

                        //Monitor.PulseAll(MainServer.flightPlans);//Sending signal to other thread
                        //Monitor.Exit(MainServer.flightPlans);//Release the lock

                    }


                    try
                    {

                        int randomFlightNumber = MainServer.random.Next(0, randomMax);
                        int countLuggage = 0;

                        Monitor.Enter(MainServer.luggageBuffer);//Locking the luggage lock
                        for (int j = 0; j < MainServer.luggageBuffer.Length; j++)
                        {
                            if ((MainServer.luggageBuffer[j] != null) && (MainServer.luggageBuffer[j].FlightNumber == MainServer.tempFlightPlans[randomFlightNumber].FlightNumber))
                            {
                                countLuggage++;
                            }
                        }


                        if ((MainServer.tempFlightPlans[randomFlightNumber] != null) && (countLuggage < MainServer.tempFlightPlans[randomFlightNumber].Seats))
                        {
                            Luggage luggage = new Luggage();
                            luggage.LuggageNumber = luggageCounter;
                            luggageCounter++;
                            luggage.PassengerNumber = paasengerNumber;
                            paasengerNumber++;
                            Faker passengerName = new Faker();
                            luggage.PassengerName = passengerName.Name.FullName();
                            luggage.FlightNumber = randomFlightNumber;

                            MainServer.luggageBuffer[MainServer.MaxLuggageBuffer - 1] = luggage;


                            // MainServer.outPut.PrintLuggage(MainServer.MaxLuggageBuffer - 1);//The output to console
                            for (int i = 0; i < MainServer.tempFlightPlans.Length; i++)
                            {
                                MainServer.tempFlightPlans[i] = null;
                            }
                        }
                    }
                    finally
                    {
                        Monitor.PulseAll(MainServer.luggageBuffer);//Sending signal to other thread
                        Monitor.Exit(MainServer.luggageBuffer);//Release the lock
                    }
                }
                Thread.Sleep(MainServer.random.Next(MainServer.randomSleepMin, MainServer.randomSleepMax));

            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class SortingUnit
    {
        #region Fields
        private int sortingUnitNumber;

        #endregion



        #region Properties
        public int SortingUnitNumber
        {
            get { return sortingUnitNumber; }
            set { sortingUnitNumber = value; }
        }

        #endregion

        #region Constructors
        public SortingUnit()
        {

        }

        #endregion

        #region Methods
        public void SortLuggage()
        {
            Luggage[] tempLuggage = new Luggage[1];//To have an object array to keep temp luggage in the mainentrance
            FlightPlan[] tempFlight = new FlightPlan[1];//temperary flightplan to keep values when moving to decide where to send luggage

            while (true)
            {

                //receive luggage from the SortingUnitbuffer

                try
                {
                    if ((MainServer.sortingUnitBuffer[0] != null) && (tempLuggage[0] == null))
                    {
                        Monitor.Enter(MainServer.sortingUnitBuffer);//Locking the thread
                        Array.Copy(MainServer.sortingUnitBuffer, 0, tempLuggage, 0, 1);//Copy first index from luggagebuffer to the temp array
                        tempLuggage[0].SortInTimeStmap = DateTime.Now;
                        MainServer.sortingUnitBuffer[0] = null;
                    }
                    else
                    {
                        Monitor.Wait(MainServer.sortingUnitBuffer);//Setting the thread in waiting state
                    };
                }
                finally
                {
                    Monitor.Pulse(MainServer.sortingUnitBuffer);//Sending signal to LuggageWorker
                    Monitor.Exit(MainServer.sortingUnitBuffer);//Unlocking thread
                };
                // MainServer.outPut.PrintSortingArrival(tempLuggage[0]);



                //Getting the right Gatenumber if templuggage and tempflight is not null and adding the luggage to the gate buffer
                if (tempLuggage[0] != null)
                {
                    //Getting the gateNumber and flightnumber for the luggage in tempbuffer
                    for (int i = 0; i < MainServer.flightPlans.Length; i++)
                    {
                        try
                        {
                            if (tempLuggage[0].FlightNumber == MainServer.flightPlans[i].FlightNumber)
                            {
                                Monitor.Enter(MainServer.flightPlans);//Locking the thread
                                Array.Copy(MainServer.flightPlans, i, tempFlight, 0, 1);//Copy first index from luggagebuffer to the temp array
                                i = MainServer.flightPlans.Length - 1;
                            };
                        }
                        finally
                        {
                            Monitor.Pulse(MainServer.flightPlans);//Sending signal to LuggageWorker
                            Monitor.Exit(MainServer.flightPlans);//Unlocking thread
                        };
                    };



                    //Moving luggage from tempbuffeer to the right gatebuffer if it fits to the selected luggage destination gatenumber if the 
                    if (tempFlight != null)
                    {
                        if (MainServer.gateBuffers[tempFlight[0].GateNumber].Buffer[MainServer.gateBufferSize - 1] == null)
                        {
                            try
                            {
                                Monitor.Enter(MainServer.gateBuffers[tempFlight[0].GateNumber]);//Locking the thread
                                tempLuggage[0].SortOutTimeStamp = DateTime.Now;//Adding a timestamp for when the luggage has exited the sorting unit
                                Array.Copy(tempLuggage, 0, MainServer.gateBuffers[tempFlight[0].GateNumber].Buffer, MainServer.gateBufferSize - 1, 1);//Copy first index from tempLuggage to the last index in the luggage buffer array
                            }
                            finally
                            {
                                Monitor.Pulse(MainServer.gateBuffers[tempFlight[0].GateNumber]);//Sending signal to gatebufferWorker
                                Monitor.Exit(MainServer.gateBuffers[tempFlight[0].GateNumber]);//Unlocking thread
                                tempFlight[0] = null;
                            };

                        }
                        else
                        {
                            Monitor.Wait(MainServer.gateBuffers[tempFlight[0].GateNumber]);//Waiting for the gate buffer to release the lock when it has emptied the last index in the array
                        }
                    }
                    else// if tempflight is == null
                    {
                        //----------------------------------------------------------------------------
                        //Getting a new gateNumber and flightnumber to relocate the luggage to if another gate has the same destination if a such exists
                        //----------------------------------------------------------------------------
                        for (int i = 0; i < MainServer.flightPlanLog.Length; i++)
                        {
                            if (tempLuggage[0].FlightNumber == MainServer.flightPlanLog[i].FlightNumber)
                            {
                                try
                                {
                                    Monitor.Enter(MainServer.flightPlanLog);//Locking the thread
                                    Array.Copy(MainServer.flightPlanLog, i, tempFlight, 0, 1);//Copy first index from luggagebuffer to the temp array
                                    i = MainServer.flightPlanLog.Length - 1;
                                }
                                finally
                                {
                                    Monitor.Pulse(MainServer.flightPlanLog);//Sending signal to LuggageWorker
                                    Monitor.Exit(MainServer.flightPlanLog);//Unlocking thread
                                };
                            };
                        };
                        //----------------------------------------------------------------------------
                        //Else relocate to sorter buffer with a new flightnumber from the flightPlans
                        //----------------------------------------------------------------------------
                    }

                };



                //----------------------------------------------------------------------------
                //Everything below here is somehow crap and need to be refactored in the above
                //----------------------------------------------------------------------------

                //If luggage is too late to the gate and the gate is closed and there is a plane in the flightplan with same destination, then redirict luggage to another gate for that destination
                if (tempLuggage[0] != null)
                {
                    // Monitor.Enter(MainServer.flightPlans);//Locking the thread
                    try
                    {
                        for (int i = 0; i < MainServer.flightPlans.Length; i++)
                        {
                            if (destination == MainServer.flightPlans[i].Destination)
                            {
                                gateNumber = MainServer.flightPlans[i].GateNumber;
                                i = MainServer.flightPlans.Length;
                            };
                        };

                    }
                    finally
                    {
                        //Monitor.Pulse(MainServer.flightPlans);//Sending signal to LuggageWorker
                        //Monitor.Exit(MainServer.flightPlans);//Unlocking thread
                    };

                    if (gateNumber != -1 && MainServer.gateBuffers[gateNumber].Buffer[MainServer.gateBufferSize - 1] == null)
                    {
                        try
                        {
                            Monitor.Enter(MainServer.gateBuffers[gateNumber]);//Locking the thread
                            Array.Copy(tempLuggage, 0, MainServer.gateBuffers[gateNumber].Buffer, MainServer.gateBufferSize - 1, 1);//Copy first index from tempLuggage to the last index in the luggage buffer array
                            MainServer.outPut.PrintSortedToGate(tempLuggage[0], gateNumber);
                            tempLuggage[0] = null;
                        }
                        finally
                        {
                            Monitor.Pulse(MainServer.gateBuffers[gateNumber]);//Sending signal to LuggageWorker
                            Monitor.Exit(MainServer.gateBuffers[gateNumber]);//Unlocking thread
                        };
                    }
                    else
                    {
                        try
                        {
                            Monitor.Enter(MainServer.sortingUnitBuffer);//Sending signal to LuggageWorker
                            if (MainServer.sortingUnitBuffer[MainServer.sortBufferSize - 1] == null)
                            {
                                Array.Copy(tempLuggage, 0, MainServer.sortingUnitBuffer, MainServer.sortBufferSize - 1, 1);//Copy first index from tempLuggage to the last index in the luggage buffer array
                                MainServer.outPut.PrintLuggageReturnedToSortingBuffer(tempLuggage[0]);//Print htat the luggage has been added to the sorting queue for next flight to that destination
                                tempLuggage[0] = null;
                            };
                            //int countLuggage = 0; ;
                            //for (int i = 0; i < MainServer.sortingUnitBuffer.Length; i++)
                            //{
                            //    if (MainServer.sortingUnitBuffer[i] != null)
                            //    {
                            //        countLuggage++;
                            //    };
                            //};
                            //MainServer.outPut.PrintSortingBufferCapacity(countLuggage);
                        }
                        finally
                        {
                            Monitor.Pulse(MainServer.sortingUnitBuffer);//Sending signal to LuggageWorker
                            Monitor.Exit(MainServer.sortingUnitBuffer);//Unlocking thread
                        };
                    };
                };
                Thread.Sleep(MainServer.random.Next(MainServer.randomSleepMin, MainServer.randomSleepMax));
            };
        }
        #endregion
    }
}

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

        #endregion

        #region Properties

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
                //----------------------------------------------------------------------------
                //receive luggage from the SortingUnitbuffer
                //----------------------------------------------------------------------------
                Monitor.Enter(MainServer.sortingUnitBuffer);//Locking the thread
                try
                {
                    if ((MainServer.sortingUnitBuffer[0] != null) && (tempLuggage[0] == null))
                    {
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



                //----------------------------------------------------------------------------
                //Setting the tempflight to fit the tempLuggage
                //----------------------------------------------------------------------------
                if (tempLuggage[0] != null)
                {
                    //Getting the gateNumber and flightnumber for the luggage in tempbuffer
                    Monitor.Enter(MainServer.flightPlans);//Locking the thread
                    try
                    {
                        for (int i = 0; i < MainServer.flightPlans.Length; i++)
                        {
                            if (MainServer.flightPlans[i] != null && tempLuggage[0].FlightNumber == MainServer.flightPlans[i].FlightNumber)
                            {
                                Array.Copy(MainServer.flightPlans, i, tempFlight, 0, 1);//Copy flightplan from the index to the tempFlight array
                                i = MainServer.flightPlans.Length - 1;
                            };
                        };
                    }
                    finally
                    {
                        Monitor.Pulse(MainServer.flightPlans);//Sending signal to LuggageWorker
                        Monitor.Exit(MainServer.flightPlans);//Unlocking thread
                    };



                    //----------------------------------------------------------------------------
                    //Moving luggage from tempbuffer to the right gatebuffer if it fits to the selected luggage destination gatenumber or if the gatebuffer is empty
                    //----------------------------------------------------------------------------
                    if (tempFlight[0] != null)
                    {
                        Monitor.Enter(MainServer.gateBuffers[tempFlight[0].GateNumber]);//Locking the thread
                        try
                        {
                            //if last index in buffer is null and gate is not yet closed
                            if (MainServer.gateBuffers[tempFlight[0].GateNumber].Buffer[MainServer.gateBufferSize - 1] == null && (tempFlight[0].DepartureTime - DateTime.Now).TotalSeconds >= MainServer.gateCloseBeforeDeparture)
                            {
                                for (int i = 0; i < MainServer.gateBuffers[tempFlight[0].GateNumber].Buffer.Length; i++)
                                {
                                    if (MainServer.gateBuffers[tempFlight[0].GateNumber].Buffer[i] != null && MainServer.gateBuffers[tempFlight[0].GateNumber].Buffer[i].FlightNumber == tempFlight[0].FlightNumber)
                                    {
                                        // tempLuggage[0].SortOutTimeStamp = DateTime.Now;//Adding a timestamp for when the luggage has exited the sorting unit
                                        Array.Copy(tempLuggage, 0, MainServer.gateBuffers[tempFlight[0].GateNumber].Buffer, MainServer.gateBufferSize - 1, 1);//Copy first index from tempLuggage to the last index in the luggage buffer array
                                        tempLuggage[0] = null;
                                        i = MainServer.gateBuffers[tempFlight[0].GateNumber].Buffer.Length;
                                    };
                                };
                                bool isNotEmpty=false;
                                for (int i = 0; i < MainServer.gateBuffers[tempFlight[0].GateNumber].Buffer.Length; i++)
                                {
                                    if (MainServer.gateBuffers[tempFlight[0].GateNumber].Buffer[i] !=null)
                                    {
                                        isNotEmpty = true;
                                    };
                                };
                                if (tempLuggage[0] != null && !isNotEmpty)// if no instances was found
                                {
                                    //  tempLuggage[0].SortOutTimeStamp = DateTime.Now;//Adding a timestamp for when the luggage has exited the sorting unit
                                    Array.Copy(tempLuggage, 0, MainServer.gateBuffers[tempFlight[0].GateNumber].Buffer, MainServer.gateBufferSize - 1, 1);//Copy first index from tempLuggage to the last index in the luggage buffer array
                                    tempLuggage[0] = null;
                                };
                            };
                        }
                        finally
                        {
                            Monitor.Pulse(MainServer.gateBuffers[tempFlight[0].GateNumber]);//Sending signal to gatebufferWorker
                            Monitor.Exit(MainServer.gateBuffers[tempFlight[0].GateNumber]);//Unlocking thread
                            tempFlight[0] = null;
                        };
                    }
                }



                //------------------------------------------------------------------------------------------------------------------------------------------
                //If the luggage did not get to a gate due to that there is already an other plane with an earlier departure or the plane has left.
                //----------------------------------------------------------------------------
                if (tempLuggage[0] != null && tempFlight[0] == null)
                {
                    //----------------------------------------------------------------------------
                    //Getting a new flightnumber and gateNumber to relocate the luggage to.
                    //----------------------------------------------------------------------------
                    Monitor.Enter(MainServer.flightPlanLog);//Locking the thread
                    try
                    {
                        for (int i = 0; i < MainServer.flightPlanLog.Length; i++)
                        {
                            if (MainServer.flightPlanLog[i] != null && tempLuggage[0].FlightNumber == MainServer.flightPlanLog[i].FlightNumber)
                            {
                                Array.Copy(MainServer.flightPlanLog, i, tempFlight, 0, 1);//Copy first index from luggagebuffer to the temp array
                                i = MainServer.flightPlanLog.Length - 1;
                            };
                        };
                    }
                    finally
                    {
                        Monitor.Pulse(MainServer.flightPlanLog);//Sending signal to LuggageWorker
                        Monitor.Exit(MainServer.flightPlanLog);//Unlocking thread
                    };


                    if (tempFlight[0] != null)
                    {
                        //Getting the new flightplan for the same destination
                        Monitor.Enter(MainServer.flightPlans);//Locking the thread
                        try
                        {
                            for (int i = 0; i < MainServer.flightPlans.Length; i++)
                            {
                                if (MainServer.flightPlans[i] != null && tempFlight[0].Destination == MainServer.flightPlans[i].Destination)
                                {
                                    Array.Copy(MainServer.flightPlans, i, tempFlight, 0, 1);//Copy first index from luggagebuffer to the temp array
                                    i = MainServer.flightPlans.Length - 1;
                                };
                            };
                        }
                        finally
                        {
                            Monitor.Pulse(MainServer.flightPlans);//Sending signal to LuggageWorker
                            Monitor.Exit(MainServer.flightPlans);//Unlocking thread
                        };

                    };


                    //----------------------------------------------------------------------------
                    //return the luggage to the SortingBuffer to be allocated to another gate
                    //----------------------------------------------------------------------------
                    if (tempFlight[0] != null)
                    {
                        Monitor.Enter(MainServer.sortingUnitBuffer);//Locking the thread
                        try
                        {
                            if (MainServer.sortingUnitBuffer[MainServer.sortingUnitBuffer.Length - 1] == null)
                            {
                                Array.Copy(tempLuggage, 0, MainServer.sortingUnitBuffer, MainServer.sortingUnitBuffer.Length - 1, 1);//Copy first index from tempLuggage to the last index in the sorting buffer array
                                tempLuggage[0] = null;
                            }
                            else
                            {
                                Monitor.Wait(MainServer.sortingUnitBuffer);//Unlocking thread
                            };
                        }
                        finally
                        {
                            Monitor.Pulse(MainServer.sortingUnitBuffer);//Sending signal to LuggageWorker
                            Monitor.Exit(MainServer.sortingUnitBuffer);//Unlocking thread
                        };
                    };
                };
                Thread.Sleep(MainServer.basicSleep);
            };
        }
        #endregion
    }
}

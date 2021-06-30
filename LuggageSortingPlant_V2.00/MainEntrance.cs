using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class MainEntrance
    {
        #region Fields
        #endregion



        #region Properties

        #endregion



        #region Constructors
        public MainEntrance()
        {

        }
        #endregion



        #region Methods
        public void SendLuggageToCheckIn()
        {
            Luggage[] tempLuggage = new Luggage[1];//To have an object array to keep temp luggage in the mainentrance

            while (true)
            {
                //--------------------------------------------------------------------------------
                //receive luggage from the hall, represented with the Luggagebuffer
                //--------------------------------------------------------------------------------
                Monitor.Enter(MainServer.luggageBuffer);//Locking the thread
                try
                {
                    if ((MainServer.luggageBuffer[0] != null) && (tempLuggage[0] == null))
                    {
                        Monitor.Enter(MainServer.luggageBuffer);//Locking the thread
                        try
                        {
                            Array.Copy(MainServer.luggageBuffer, 0, tempLuggage, 0, 1);//Copy first index from luggagebuffer to the temp array
                            MainServer.luggageBuffer[0] = null;
                            //  MainServer.outPut.PrintArrivedToTheAirport(tempLuggage[0]);
                        }
                        finally
                        {
                            Monitor.Pulse(MainServer.luggageBuffer);//Sending signal to LuggageWorker
                            Monitor.Exit(MainServer.luggageBuffer);//Unlocking thread
                        };
                    };

                }
                finally
                {
                    Monitor.Pulse(MainServer.luggageBuffer);//Sending signal to LuggageWorker
                    Monitor.Exit(MainServer.luggageBuffer);//Unlocking thread
                }

                for (int i = 0; i < MainServer.checkIns.Length; i++)
                {
                    Monitor.Enter(MainServer.checkIns[i]);//Locking the thread
                    Monitor.Enter(MainServer.checkInBuffers[i]);//Locking the thread
                    try
                    {
                        if (MainServer.checkIns[i].CheckInForFlight[0] != null && MainServer.checkIns[i].CheckInForFlight[0].FlightNumber == tempLuggage[0].FlightNumber)
                        {
                            if (MainServer.checkInBuffers[i].Buffer[MainServer.checkInBufferSize - 1] == null)
                            {
                                if (((MainServer.checkIns[i].CheckInForFlight[0].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.checkInOpenBeforeDeparture) && ((MainServer.checkIns[i].CheckInForFlight[0].DepartureTime - DateTime.Now).TotalSeconds >= MainServer.checkInCloseBeforeDeparture))
                                {
                                    Array.Copy(tempLuggage, 0, MainServer.checkInBuffers[i].Buffer, MainServer.checkInBufferSize - 1, 1);//Copy first index from tempArray to the checkin buffer
                                    tempLuggage[0] = null;
                                }
                            }
                        }
                    }
                    finally
                    {
                        Monitor.Pulse(MainServer.checkIns[i]);//Sending signal to LuggageWorker
                        Monitor.Exit(MainServer.checkIns[i]);//Unlocking thread
                        Monitor.Pulse(MainServer.checkInBuffers[i]);//Sending signal to LuggageWorker
                        Monitor.Exit(MainServer.checkInBuffers[i]);//Unlocking thread
                    }
                }

                //This is a life hack----------------------------------------------------
                if (tempLuggage[0] != null)
                {
                    MainServer.luggageWhoMissedThePlane.Add(tempLuggage[0]); //Adding the luggage to the list that contains those who were late to checkin (There wasn't time to relocate them
                };






                //---------------------------------------OLD VERSION THAT DID NOT WORK AS SUPPOSED TO---------------------------------------------------------------------
                ////--------------------------------------------------------------------------------
                ////Check if there is already a buffer in use but its not full for the specific flight and if thats the case, insert luggae object in that buffer
                ////--------------------------------------------------------------------------------
                // for (int i = 0; i < MainServer.checkInBuffers.Length; i++)//Run throught all the buffers in the array
                // {
                //    //checkInNumber = i;
                //    if (MainServer.checkInBuffers[i].Buffer[MainServer.checkInBufferSize - 1] == null)
                //    {
                //        Monitor.Enter(MainServer.checkInBuffers[i]);//Locking the thread
                //        try
                //        {
                //            for (int j = 0; j < MainServer.checkInBuffers[i].Buffer.Length; j++)//loop through the current Buffer
                //            {
                //                if (tempLuggage[0] != null && MainServer.checkInBuffers[i].Buffer[j] != null)//If templuggage, and buffer index "j" in buffer "i" is not null
                //                {
                //                    //int luggageCount = 0;
                //                    if ((j < MainServer.checkInBuffers[i].Buffer.Length - 1) && (tempLuggage[0].FlightNumber == MainServer.checkInBuffers[i].Buffer[j].FlightNumber)) //If buffer is not full and If Luggage flightnumber is = flightNumber in the buffer
                //                    {
                //                        Array.Copy(tempLuggage, 0, MainServer.checkInBuffers[i].Buffer, MainServer.checkInBufferSize - 1, 1);//Copy first index from tempLuggage to the last index in the checkIn buffer array
                //                        tempLuggage[0] = null;
                //                        j = MainServer.checkInBuffers[i].Buffer.Length - 1;
                //                    };
                //                };
                //            };
                //        }
                //        finally
                //        {
                //            Monitor.PulseAll(MainServer.checkInBuffers[i]);//Sending signal to LuggageWorker
                //            Monitor.Exit(MainServer.checkInBuffers[i]);//Unlocking thread
                //            if (tempLuggage[0] == null)
                //            {
                //                i = MainServer.checkInBuffers.Length - 1;
                //            };
                //        };
                //    };
                //};


                ////--------------------------------------------------------------------------------
                ////If there is no buffer already in use for this gate and luggage object is still not null after first check,
                ////then  if there is an empty buffer, then begin using that one instaed.
                ////--------------------------------------------------------------------------------
                //if (tempLuggage[0] != null)
                //{
                //    for (int i = 0; i < MainServer.checkInBuffers.Length; i++)
                //    {
                //        Monitor.Enter(MainServer.checkInBuffers[i]);//Locking the thread
                //        try
                //        {
                //            bool containsLuggage = false;
                //            for (int j = 0; j < MainServer.checkInBuffers[i].Buffer.Length; j++)//loop through the current Buffer
                //            {
                //                if (MainServer.checkInBuffers[i].Buffer[j] != null)//Check if the buffer contains anything
                //                {
                //                    containsLuggage = true;
                //                    j = MainServer.checkInBuffers[i].Buffer.Length - 1;
                //                };
                //            };
                //            if (!containsLuggage)//if the buffer is empty
                //            {
                //                Array.Copy(tempLuggage, 0, MainServer.checkInBuffers[i].Buffer, MainServer.checkInBufferSize - 1, 1);//Copy first index from tempLuggage to the last index in the current checkIn buffer array
                //                tempLuggage[0] = null;//Emptying the templuggageBuffer
                //            };
                //        }
                //        finally
                //        {
                //            Monitor.PulseAll(MainServer.checkInBuffers[i]);//Sending signal to LuggageWorker
                //            Monitor.Exit(MainServer.checkInBuffers[i]);//Unlocking thread
                //        };
                //        if (tempLuggage[0] == null)
                //        {
                //            i = MainServer.checkInBuffers.Length - 1;
                //        };
                //    };
                //};


                ////--------------------------------------------------------------------------------
                ////If luggage object is still not null after first and secund check, then return the luggage
                ////to the luggage buffer to be addressed to another flight to same destination
                ////--------------------NOT WORKING-----------------------------------------------
                ////--------------------------------------------------------------------------------
                //if (tempLuggage[0] != null)
                //{
                //    Monitor.Enter(MainServer.luggageBuffer);//Locking the thread
                //    try
                //    {
                //        for (int i = 0; i < MainServer.flightPlans.Length; i++)
                //        {
                //            if (tempLuggage[0].FlightNumber == MainServer.flightPlans[i].FlightNumber)
                //            {
                //                for (int j = 0; j < MainServer.flightPlans.Length; j++)
                //                {
                //                    if (MainServer.flightPlans[i].Destination == MainServer.flightPlans[j].Destination && MainServer.flightPlans[i].FlightNumber != MainServer.flightPlans[j].FlightNumber)
                //                    {
                //                        tempLuggage[0].FlightNumber = MainServer.flightPlans[j].FlightNumber;
                //                    }
                //                }
                //            }
                //        }
                //        if (MainServer.luggageBuffer[MainServer.MaxLuggageBuffer - 1] == null)
                //        {
                //            Array.Copy(tempLuggage, 0, MainServer.luggageBuffer, MainServer.MaxLuggageBuffer - 1, 1);//Copy first index from tempLuggage to the last index in the luggage buffer array
                //            tempLuggage[0] = null;
                //        }
                //        //else
                //        //{
                //        //    Monitor.Wait(MainServer.luggageBuffer);//Setting the thread in waiting state
                //        //};
                //    }
                //    finally
                //    {
                //        Monitor.Pulse(MainServer.luggageBuffer);//Sending signal to LuggageWorker
                //        Monitor.Exit(MainServer.luggageBuffer);//Unlocking thread
                //    };
                //};



                Thread.Sleep(MainServer.random.Next(MainServer.randomSleepMin, MainServer.randomSleepMax));
                //Thread.Sleep(MainServer.BasicSleep);
            };
        }
        #endregion
    }
}

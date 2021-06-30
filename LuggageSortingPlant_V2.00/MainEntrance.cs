using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

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
                    if (MainServer.luggageBuffer[0] != null)
                    {
                        if (tempLuggage[0] == null)
                        {
                            Array.Copy(MainServer.luggageBuffer, 0, tempLuggage, 0, 1);//Copy first index from luggagebuffer to the temp array
                            MainServer.luggageBuffer[0] = null;
                        }
                    }
                    //else
                    //{
                    //    Monitor.Wait(MainServer.luggageBuffer);//Sending signal to LuggageWorker
                    //}
                }
                finally
                {
                    Monitor.Pulse(MainServer.luggageBuffer);//Sending signal to LuggageWorker
                    Monitor.Exit(MainServer.luggageBuffer);//Unlocking thread
                };


                //--------------------------------------------------------------------------------
                //Send luggage from the luggageBuffer, to the targeted checkin buffer
                //--------------------------------------------------------------------------------
                for (int i = 0; i < MainServer.checkInBuffers.Length; i++)
                {
                    if (tempLuggage[0] != null && MainServer.checkIns[i].CheckInForFlight[0] != null)
                    {


                        Monitor.Enter(MainServer.checkInBuffers[i]);//Locking the thread
                        try
                        {
                            //if (MainServer.checkInBuffers[i].Buffer[MainServer.checkInBufferSize - 1] != null)
                            //{
                            //    Monitor.Wait(MainServer.checkInBuffers[i].Buffer);
                            //}
                            if (tempLuggage[0].FlightNumber == MainServer.checkIns[i].CheckInForFlight[0].FlightNumber)
                            {
                                if (MainServer.checkInBuffers[i].Buffer[MainServer.checkInBufferSize - 1] == null)
                                {
                                    if ((MainServer.checkIns[i].CheckInForFlight[0].DepartureTime - DateTime.Now).TotalSeconds >= MainServer.checkInCloseBeforeDeparture + 5)
                                    {
                                        Debug.WriteLine(tempLuggage[0].FlightNumber);
                                        Array.Copy(tempLuggage, 0, MainServer.checkInBuffers[i].Buffer, MainServer.checkInBufferSize - 1, 1);//Copy first index from tempArray to the checkin buffer
                                        Debug.WriteLine(MainServer.checkInBuffers[i].Buffer[MainServer.checkInBufferSize - 1].FlightNumber);
                                        tempLuggage[0] = null;
                                    };
                                }


                                //else
                                //{
                                //    Monitor.Wait(MainServer.checkInBuffers[i].Buffer);
                                //};
                            };

                        }
                        finally
                        {
                            Monitor.Pulse(MainServer.checkInBuffers[i]);//Sending signal to LuggageWorker
                            Monitor.Exit(MainServer.checkInBuffers[i]);//Unlocking thread
                        };
                    };
                };

                //If luggage is somehow too late.we will put in under the carpet----------------------------------------------------
                if (tempLuggage[0] != null)
                {
                    MainServer.luggageWhoMissedThePlane.Add(tempLuggage[0]); //Adding the luggage to the list that contains those who were late to checkin (There wasn't time to relocate them
                    tempLuggage[0] = null;
                };



                Thread.Sleep(MainServer.random.Next(MainServer.randomSleepMin, MainServer.randomSleepMax));
                //Thread.Sleep(MainServer.basicSleep);
            };
        }
        #endregion
    }
}

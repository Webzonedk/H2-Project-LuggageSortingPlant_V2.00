using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
//---------------------------------------------------------------------------------
//This class represents the CheckIns opening when the flights in the checkin buffer is about to take off within a certain amount of time
//---------------------------------------------------------------------------------
namespace LuggageSortingPlant_V2._00
{
    class CheckIn
    {
        #region Fields
        private bool open;
        private int checkInNumber;
        private FlightPlan[] checkInForFlight = new FlightPlan[1];

        #endregion



        #region Properties

        public bool Open
        {
            get { return open; }
            set { open = value; }
        }
        public int CheckInNumber
        {
            get { return checkInNumber; }
            set { checkInNumber = value; }
        }



        public FlightPlan[] CheckInForFlight
        {
            get { return checkInForFlight; }
            set { checkInForFlight = value; }
        }

        #endregion



        #region Constructors
        public CheckIn()
        {

        }
        public CheckIn(bool open, int checkInNumber)
        {
            this.open = open;
            this.checkInNumber = checkInNumber;
        }
        #endregion



        #region Methods

        public void CheckInLuggage()
        {
            Luggage[] tempLuggage = new Luggage[1];//To have an object array to keep temp luggage in the mainentrance

            while (true)
            {
                // DateTime departure;
                Monitor.Enter(MainServer.checkInBuffers[CheckInNumber]);//Locking the thread
                Monitor.Enter(MainServer.checkIns[CheckInNumber]);//Locking the thread
                try
                {
                    if (CheckInForFlight[0] != null)
                    {
                        if (((CheckInForFlight[0].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.checkInOpenBeforeDeparture) && ((CheckInForFlight[0].DepartureTime - DateTime.Now).TotalSeconds >= MainServer.checkInCloseBeforeDeparture))
                        {
                            Open = true;
                        }
                        if ((CheckInForFlight[0].DepartureTime - DateTime.Now).TotalSeconds < MainServer.checkInCloseBeforeDeparture)
                        {
                            Open = false;
                            CheckInForFlight[0] = null;
                        }
                    }
                    else
                    {
                        Open = false;
                    }

                    if (Open && MainServer.checkInBuffers[CheckInNumber].Buffer[0] != null)
                    {
                        //removing luggage from the checkIn buffer
                        Array.Copy(MainServer.checkInBuffers[CheckInNumber].Buffer, 0, tempLuggage, 0, 1);//Copy first index from checkIn buffer to the temp array
                        tempLuggage[0].CheckInTimeStamp = DateTime.Now;
                        // MainServer.outPut.PrintCheckInArrival(tempLuggage[0]);
                        MainServer.checkInBuffers[CheckInNumber].Buffer[0] = null;
                    }
                }
                finally
                {
                    Monitor.PulseAll(MainServer.checkInBuffers[CheckInNumber]);//Sending signal to other thread
                    Monitor.Exit(MainServer.checkInBuffers[CheckInNumber]);//Release the lock
                    Monitor.PulseAll(MainServer.checkIns[CheckInNumber]);//Sending signal to other thread
                    Monitor.Exit(MainServer.checkIns[CheckInNumber]);//Release the lock
                };





                // Adding luggage to SortingBuffer
                Monitor.Enter(MainServer.sortingUnitBuffer);//Locking the thread
                try
                {
                    if ((MainServer.sortingUnitBuffer[MainServer.sortBufferSize - 1] == null) && (tempLuggage[0] != null))
                    {
                        Array.Copy(tempLuggage, 0, MainServer.sortingUnitBuffer, MainServer.sortBufferSize - 1, 1);//Copy first index from checkIn buffer to the temp array
                        tempLuggage[0] = null;
                    };
                }
                finally
                {
                    Monitor.PulseAll(MainServer.sortingUnitBuffer);//Sending signal to other thread
                    Monitor.Exit(MainServer.sortingUnitBuffer);//Release the lock
                };












                //for (int i = 0; i < MainServer.checkInBuffers[CheckInNumber].Buffer.Length; i++)
                //{
                //    if (MainServer.checkInBuffers[CheckInNumber].Buffer[i] != null)
                //    {
                //        Monitor.Enter(MainServer.flightPlans);//Locking the thread
                //        try
                //        {
                //            //find flight in flightplan and get departuretime if the luggage is not too late to the flight
                //            for (int j = 0; j < MainServer.flightPlans.Length; j++)
                //            {
                //                if (MainServer.flightPlans[j] != null && MainServer.checkInBuffers[CheckInNumber].Buffer[i].FlightNumber == MainServer.flightPlans[j].FlightNumber)
                //                {
                //                    // departure = MainServer.flightPlans[i].DepartureTime;//getting the depaturtime to use to open checkin
                //                    if (((MainServer.flightPlans[j].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.checkInOpenBeforeDeparture) && ((MainServer.flightPlans[j].DepartureTime - DateTime.Now).TotalSeconds >= MainServer.checkInCloseBeforeDeparture))
                //                    {
                //                        Open = true;
                //                    }
                //                    else
                //                    {
                //                        Open = false;
                //                    };
                //                    j = MainServer.flightPlans.Length - 1;
                //                };

                //            };

                //        }
                //        finally
                //        {
                //            Monitor.PulseAll(MainServer.flightPlans);//Sending signal to other thread
                //            Monitor.Exit(MainServer.flightPlans);//Release the lock
                //        }

                //        Monitor.Enter(MainServer.flightPlanLog);//Locking the thread
                //        try
                //        {
                //            for (int j = 0; j < MainServer.flightPlanLog.Length; j++)
                //            {
                //                if (MainServer.flightPlanLog[j] != null && MainServer.checkInBuffers[CheckInNumber].Buffer[i].FlightNumber == MainServer.flightPlanLog[j].FlightNumber)
                //                {
                //                    if ((MainServer.flightPlanLog[j].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.checkInCloseBeforeDeparture)
                //                    {
                //                        Open = false;
                //                    };
                //                    j = MainServer.flightPlanLog.Length - 1;
                //                };
                //            };
                //        }
                //        finally
                //        {
                //            Monitor.PulseAll(MainServer.flightPlanLog);//Sending signal to other thread
                //            Monitor.Exit(MainServer.flightPlanLog);//Release the lock
                //        };



                //        if (Open && (MainServer.checkInBuffers[CheckInNumber].Buffer[i] != null) && tempLuggage[0] == null)// If open
                //        {
                //            //removing luggage from the checkIn buffer
                //            Array.Copy(MainServer.checkInBuffers[CheckInNumber].Buffer, i, tempLuggage, 0, 1);//Copy first index from checkIn buffer to the temp array
                //            tempLuggage[0].CheckInTimeStamp = DateTime.Now;
                //            // MainServer.outPut.PrintCheckInArrival(tempLuggage[0]);
                //            MainServer.checkInBuffers[CheckInNumber].Buffer[i] = null;
                //        };


                //        if (!Open)// If open
                //        {
                //            Monitor.Enter(MainServer.flightPlanLog);//Locking the thread
                //            try
                //            {
                //                for (int j = 0; j < MainServer.flightPlanLog.Length; j++)
                //                {
                //                    if (MainServer.checkInBuffers[CheckInNumber].Buffer[i] != null && MainServer.flightPlanLog[j] != null && MainServer.checkInBuffers[CheckInNumber].Buffer[i].FlightNumber == MainServer.flightPlanLog[j].FlightNumber)
                //                    {
                //                        if ((MainServer.flightPlanLog[j].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.checkInCloseBeforeDeparture)
                //                        {
                //                            if (tempLuggage[0] == null)
                //                            {
                //                                //Array.Copy(MainServer.checkInBuffers[CheckInNumber].Buffer, 0, tempLuggage, 0, 1);//Copy first index from checkIn buffer to the temp array
                //                                MainServer.luggageWhoMissedThePlane.Add(MainServer.checkInBuffers[CheckInNumber].Buffer[i]); //Adding the luggage to the list that contains those who were late to checkin (There wasn't time to relocate them
                //                                MainServer.checkInBuffers[CheckInNumber].Buffer[i] = null;
                //                            };
                //                        };
                //                        j = MainServer.flightPlanLog.Length - 1;
                //                    };
                //                };
                //            }
                //            finally
                //            {
                //                Monitor.PulseAll(MainServer.flightPlanLog);//Sending signal to other thread
                //                Monitor.Exit(MainServer.flightPlanLog);//Release the lock
                //            };
                //            //removing luggage from the checkIn buffer
                //            //if ((MainServer.checkInBuffers[CheckInNumber].Buffer[0] != null) && tempLuggage[0] == null)
                //            //{
                //            //    Array.Copy(MainServer.checkInBuffers[CheckInNumber].Buffer, 0, tempLuggage, 0, 1);//Copy first index from checkIn buffer to the temp array
                //            //    MainServer.luggageWhoMissedThePlane.Add(MainServer.checkInBuffers[CheckInNumber].Buffer[0]); //Adding the luggage to the list that contains those who were late to checkin (There wasn't time to relocate them
                //            //    MainServer.checkInBuffers[CheckInNumber].Buffer[0] = null;
                //            //};
                //        };
                //    }
                //}






                ////Adding luggage to SortingBuffer
                //Monitor.Enter(MainServer.sortingUnitBuffer);//Locking the thread
                //try
                //{
                //    if ((MainServer.sortingUnitBuffer[MainServer.sortBufferSize - 1] == null) && (tempLuggage[0] != null))
                //    {
                //        Array.Copy(tempLuggage, 0, MainServer.sortingUnitBuffer, MainServer.sortBufferSize - 1, 1);//Copy first index from checkIn buffer to the temp array
                //        tempLuggage[0] = null;
                //    };
                //}
                //finally
                //{
                //    Monitor.PulseAll(MainServer.sortingUnitBuffer);//Sending signal to other thread
                //    Monitor.Exit(MainServer.sortingUnitBuffer);//Release the lock
                //};












                ////----------------------------------------------------------------------------------------------------
                ////----------------------------------------------------------------------------------------------------
                ////--------------------------OLD VERSION, NOT VORKING-------------------------------------------
                ////----------------------------------------------------------------------------------------------------

                //// DateTime departure;
                //Monitor.Enter(MainServer.checkInBuffers[CheckInNumber]);//Locking the thread
                //try
                //{

                //    for (int i = 0; i < MainServer.checkInBuffers[CheckInNumber].Buffer.Length; i++)
                //    {
                //        if (MainServer.checkInBuffers[CheckInNumber].Buffer[i] != null)
                //        {
                //            Monitor.Enter(MainServer.flightPlans);//Locking the thread
                //            try
                //            {
                //                //find flight in flightplan and get departuretime if the luggage is not too late to the flight
                //                for (int j = 0; j < MainServer.flightPlans.Length; j++)
                //                {
                //                    if (MainServer.flightPlans[j] != null && MainServer.checkInBuffers[CheckInNumber].Buffer[i].FlightNumber == MainServer.flightPlans[j].FlightNumber)
                //                    {
                //                        // departure = MainServer.flightPlans[i].DepartureTime;//getting the depaturtime to use to open checkin
                //                        if (((MainServer.flightPlans[j].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.checkInOpenBeforeDeparture) && ((MainServer.flightPlans[j].DepartureTime - DateTime.Now).TotalSeconds >= MainServer.checkInCloseBeforeDeparture))
                //                        {
                //                            Open = true;
                //                        }
                //                        else
                //                        {
                //                            Open = false;
                //                        };
                //                        j = MainServer.flightPlans.Length - 1;
                //                    };

                //                };

                //            }
                //            finally
                //            {
                //                Monitor.PulseAll(MainServer.flightPlans);//Sending signal to other thread
                //                Monitor.Exit(MainServer.flightPlans);//Release the lock
                //            }

                //            Monitor.Enter(MainServer.flightPlanLog);//Locking the thread
                //            try
                //            {
                //                for (int j = 0; j < MainServer.flightPlanLog.Length; j++)
                //                {
                //                    if (MainServer.flightPlanLog[j] != null && MainServer.checkInBuffers[CheckInNumber].Buffer[i].FlightNumber == MainServer.flightPlanLog[j].FlightNumber)
                //                    {
                //                        if ((MainServer.flightPlanLog[j].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.checkInCloseBeforeDeparture)
                //                        {
                //                            Open = false;
                //                        };
                //                        j = MainServer.flightPlanLog.Length - 1;
                //                    };
                //                };
                //            }
                //            finally
                //            {
                //                Monitor.PulseAll(MainServer.flightPlanLog);//Sending signal to other thread
                //                Monitor.Exit(MainServer.flightPlanLog);//Release the lock
                //            };



                //            if (Open && (MainServer.checkInBuffers[CheckInNumber].Buffer[i] != null) && tempLuggage[0] == null)// If open
                //            {
                //                //removing luggage from the checkIn buffer
                //                Array.Copy(MainServer.checkInBuffers[CheckInNumber].Buffer, i, tempLuggage, 0, 1);//Copy first index from checkIn buffer to the temp array
                //                tempLuggage[0].CheckInTimeStamp = DateTime.Now;
                //                // MainServer.outPut.PrintCheckInArrival(tempLuggage[0]);
                //                MainServer.checkInBuffers[CheckInNumber].Buffer[i] = null;
                //            };


                //            if (!Open)// If open
                //            {
                //                Monitor.Enter(MainServer.flightPlanLog);//Locking the thread
                //                try
                //                {
                //                    for (int j = 0; j < MainServer.flightPlanLog.Length; j++)
                //                    {
                //                        if (MainServer.checkInBuffers[CheckInNumber].Buffer[i] != null && MainServer.flightPlanLog[j] != null && MainServer.checkInBuffers[CheckInNumber].Buffer[i].FlightNumber == MainServer.flightPlanLog[j].FlightNumber)
                //                        {
                //                            if ((MainServer.flightPlanLog[j].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.checkInCloseBeforeDeparture)
                //                            {
                //                                if (tempLuggage[0] == null)
                //                                {
                //                                    //Array.Copy(MainServer.checkInBuffers[CheckInNumber].Buffer, 0, tempLuggage, 0, 1);//Copy first index from checkIn buffer to the temp array
                //                                    MainServer.luggageWhoMissedThePlane.Add(MainServer.checkInBuffers[CheckInNumber].Buffer[i]); //Adding the luggage to the list that contains those who were late to checkin (There wasn't time to relocate them
                //                                    MainServer.checkInBuffers[CheckInNumber].Buffer[i] = null;
                //                                };
                //                            };
                //                            j = MainServer.flightPlanLog.Length - 1;
                //                        };
                //                    };
                //                }
                //                finally
                //                {
                //                    Monitor.PulseAll(MainServer.flightPlanLog);//Sending signal to other thread
                //                    Monitor.Exit(MainServer.flightPlanLog);//Release the lock
                //                };
                //                //removing luggage from the checkIn buffer
                //                //if ((MainServer.checkInBuffers[CheckInNumber].Buffer[0] != null) && tempLuggage[0] == null)
                //                //{
                //                //    Array.Copy(MainServer.checkInBuffers[CheckInNumber].Buffer, 0, tempLuggage, 0, 1);//Copy first index from checkIn buffer to the temp array
                //                //    MainServer.luggageWhoMissedThePlane.Add(MainServer.checkInBuffers[CheckInNumber].Buffer[0]); //Adding the luggage to the list that contains those who were late to checkin (There wasn't time to relocate them
                //                //    MainServer.checkInBuffers[CheckInNumber].Buffer[0] = null;
                //                //};
                //            };
                //        }
                //    }
                //}
                //finally
                //{
                //    Monitor.PulseAll(MainServer.checkInBuffers[CheckInNumber]);//Sending signal to other thread
                //    Monitor.Exit(MainServer.checkInBuffers[CheckInNumber]);//Release the lock
                //};





                ////Adding luggage to SortingBuffer
                //Monitor.Enter(MainServer.sortingUnitBuffer);//Locking the thread
                //try
                //{
                //    if ((MainServer.sortingUnitBuffer[MainServer.sortBufferSize - 1] == null) && (tempLuggage[0] != null))
                //    {
                //        Array.Copy(tempLuggage, 0, MainServer.sortingUnitBuffer, MainServer.sortBufferSize - 1, 1);//Copy first index from checkIn buffer to the temp array
                //        tempLuggage[0] = null;
                //    };
                //}
                //finally
                //{
                //    Monitor.PulseAll(MainServer.sortingUnitBuffer);//Sending signal to other thread
                //    Monitor.Exit(MainServer.sortingUnitBuffer);//Release the lock
                //};








                ////----------------------------------------------------------------------------------------------------
                ////----------------------------------------------------------------------------------------------------
                ////--------------------------EVEN OLD VERSION, NOT VORKING EITHER-------------------------------------------
                ////----------------------------------------------------------------------------------------------------


                //// DateTime departure;
                //Monitor.Enter(MainServer.checkInBuffers[CheckInNumber]);//Locking the thread
                //try
                //{
                //    for (int i = 0; i < MainServer.checkInBuffers[CheckInNumber].Buffer.Length; i++)
                //    {

                //        if (MainServer.checkInBuffers[CheckInNumber].Buffer[i] != null)
                //        {
                //            Monitor.Enter(MainServer.flightPlans);//Locking the thread
                //            try
                //            {
                //                //find flight in flightplan and get departuretime if the luggage is not too late to the flight
                //                for (int j = 0; j < MainServer.flightPlans.Length; j++)
                //                {
                //                    if (MainServer.flightPlans[j] != null && MainServer.checkInBuffers[CheckInNumber].Buffer[i].FlightNumber == MainServer.flightPlans[j].FlightNumber)
                //                    {
                //                        // departure = MainServer.flightPlans[i].DepartureTime;//getting the depaturtime to use to open checkin
                //                        if (((MainServer.flightPlans[j].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.checkInOpenBeforeDeparture) && ((MainServer.flightPlans[j].DepartureTime - DateTime.Now).TotalSeconds >= MainServer.checkInCloseBeforeDeparture))
                //                        {
                //                            Open = true;
                //                        }
                //                        else
                //                        {
                //                            Open = false;
                //                        };
                //                        j = MainServer.flightPlans.Length - 1;
                //                    };

                //                };

                //            }
                //            finally
                //            {
                //                Monitor.PulseAll(MainServer.flightPlans);//Sending signal to other thread
                //                Monitor.Exit(MainServer.flightPlans);//Release the lock

                //            }

                //            Monitor.Enter(MainServer.flightPlanLog);//Locking the thread
                //            try
                //            {
                //                for (int j = 0; j < MainServer.flightPlanLog.Length; j++)
                //                {
                //                    if (MainServer.flightPlanLog[j] != null && MainServer.checkInBuffers[CheckInNumber].Buffer[i].FlightNumber == MainServer.flightPlanLog[j].FlightNumber)
                //                    {
                //                        if ((MainServer.flightPlanLog[j].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.checkInCloseBeforeDeparture)
                //                        {
                //                            Open = false;
                //                        };
                //                        j = MainServer.flightPlanLog.Length - 1;
                //                    };
                //                };

                //            }
                //            finally
                //            {
                //                Monitor.PulseAll(MainServer.flightPlanLog);//Sending signal to other thread
                //                Monitor.Exit(MainServer.flightPlanLog);//Release the lock
                //            }



                //            if (Open)// If open
                //            {
                //                //removing luggage from the checkIn buffer
                //                if ((MainServer.checkInBuffers[CheckInNumber].Buffer[0] != null) && tempLuggage[0] == null)
                //                {
                //                    Array.Copy(MainServer.checkInBuffers[CheckInNumber].Buffer, 0, tempLuggage, 0, 1);//Copy first index from checkIn buffer to the temp array
                //                    tempLuggage[0].CheckInTimeStamp = DateTime.Now;
                //                    // MainServer.outPut.PrintCheckInArrival(tempLuggage[0]);
                //                    MainServer.checkInBuffers[CheckInNumber].Buffer[0] = null;
                //                };
                //            };


                //            if (!Open)// If open
                //            {
                //                //removing luggage from the checkIn buffer
                //                if ((MainServer.checkInBuffers[CheckInNumber].Buffer[0] != null) && tempLuggage[0] == null)
                //                {
                //                    Array.Copy(MainServer.checkInBuffers[CheckInNumber].Buffer, 0, tempLuggage, 0, 1);//Copy first index from checkIn buffer to the temp array
                //                    MainServer.luggageWhoMissedThePlane.Add(MainServer.checkInBuffers[CheckInNumber].Buffer[i]); //Adding the luggage to the list that contains those who were late to checkin (There wasn't time to relocate them
                //                    MainServer.checkInBuffers[CheckInNumber].Buffer[0] = null;
                //                };
                //            };



                //            i = MainServer.checkInBuffers[CheckInNumber].Buffer.Length - 1;
                //        }
                //        //else
                //        //{
                //        //    Monitor.Wait(MainServer.checkInBuffers[CheckInNumber]);//Locking the thread
                //        //};
                //    }

                //    //if (i == MainServer.checkInBuffers[CheckInNumber].Buffer.Length - 1)
                //    //{
                //    //    Open = false;
                //    //}


                //}
                //finally
                //{
                //    Monitor.PulseAll(MainServer.checkInBuffers[CheckInNumber]);//Sending signal to other thread
                //    Monitor.Exit(MainServer.checkInBuffers[CheckInNumber]);//Release the lock
                //};





                ////Adding luggage to SortingBuffer
                //Monitor.Enter(MainServer.sortingUnitBuffer);//Locking the thread
                //try
                //{
                //    if ((MainServer.sortingUnitBuffer[MainServer.sortBufferSize - 1] == null) && (tempLuggage[0] != null))
                //    {
                //        Array.Copy(tempLuggage, 0, MainServer.sortingUnitBuffer, MainServer.sortBufferSize - 1, 1);//Copy first index from checkIn buffer to the temp array
                //        tempLuggage[0] = null;
                //    };
                //}
                //finally
                //{
                //    Monitor.PulseAll(MainServer.sortingUnitBuffer);//Sending signal to other thread
                //    Monitor.Exit(MainServer.sortingUnitBuffer);//Release the lock
                //};


                ////----------------------------------------------------------------------------------------------------
                ////----------------------------------------------------------------------------------------------------
                ////----------------------------------------------------------------------------------------------------
                ////----------------------------------------------------------------------------------------------------



                //Thread.Sleep(MainServer.random.Next(MainServer.randomSleepMin, MainServer.randomSleepMax));
                Thread.Sleep(MainServer.basicSleep);
            };
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class CheckIn
    {
        #region Fields
        private bool open;
        private int checkInNumber;

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
                try
                {

                    if (MainServer.checkInBuffers[CheckInNumber].Buffer[0] != null)
                    {
                        //find flight in flightplan and get departuretime
                        for (int i = 0; i < MainServer.flightPlans.Length; i++)
                        {
                            if (MainServer.flightPlans[i] !=null && MainServer.flightPlans[i].FlightNumber == MainServer.checkInBuffers[CheckInNumber].Buffer[0].FlightNumber)
                            {
                               // departure = MainServer.flightPlans[i].DepartureTime;//getting the depaturtime to use to open checkin
                                if (((MainServer.flightPlans[i].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.checkInOpenBeforeDeparture) && ((MainServer.flightPlans[i].DepartureTime - DateTime.Now).TotalSeconds >= MainServer.checkInCloseBeforeDeparture))
                                {
                                    Open = true;
                                };
                                if ((MainServer.flightPlans[i].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.checkInCloseBeforeDeparture)
                                {
                                    Open = false;
                                };
                                i = MainServer.flightPlans.Length;
                            };

                        };


                        if (Open)// If open
                        {
                            //removing luggage from the checkIn buffer
                            if ((MainServer.checkInBuffers[CheckInNumber].Buffer[0] != null) && tempLuggage[0] == null)
                            {
                                Array.Copy(MainServer.checkInBuffers[CheckInNumber].Buffer, 0, tempLuggage, 0, 1);//Copy first index from checkIn buffer to the temp array
                                tempLuggage[0].CheckInTimeStamp = DateTime.Now;
                               // MainServer.outPut.PrintCheckInArrival(tempLuggage[0]);
                                MainServer.checkInBuffers[CheckInNumber].Buffer[0] = null;
                            };
                        };
                    }
                    else
                    {
                        Monitor.Wait(MainServer.checkInBuffers[CheckInNumber]);//Locking the thread
                    };
                }
                finally
                {
                    Monitor.PulseAll(MainServer.checkInBuffers[CheckInNumber]);//Sending signal to other thread
                    Monitor.Exit(MainServer.checkInBuffers[CheckInNumber]);//Release the lock
                };





                //Adding luggage to SortingBuffer
                try
                {
                    Monitor.Enter(MainServer.sortingUnitBuffer);//Locking the thread
                    if ((MainServer.sortingUnitBuffer[MainServer.sortBufferSize - 1] == null) && (tempLuggage[0] != null))
                    {
                        Array.Copy(tempLuggage, 0, MainServer.sortingUnitBuffer, MainServer.sortBufferSize - 1, 1);//Copy first index from checkIn buffer to the temp array
                        tempLuggage[0] = null;
                        //int countLuggage = 0;

                        //for (int i = 0; i < MainServer.sortingUnitBuffer.Length; i++)
                        //{
                        //    if (MainServer.sortingUnitBuffer[i] != null)
                        //    {
                        //        countLuggage++;
                        //    };
                        //};
                       // MainServer.outPut.PrintSortingBufferCapacity(countLuggage);
                        //}
                        //else
                        //{
                        //    Monitor.Wait(MainServer.sortingUnitBuffer);//Set thread to wait state
                    };
                }
                finally
                {
                    Monitor.PulseAll(MainServer.sortingUnitBuffer);//Sending signal to other thread
                    Monitor.Exit(MainServer.sortingUnitBuffer);//Release the lock
                };
                Thread.Sleep(MainServer.random.Next(MainServer.randomSleepMin, MainServer.randomSleepMax));
            };
        }
        #endregion
    }
}

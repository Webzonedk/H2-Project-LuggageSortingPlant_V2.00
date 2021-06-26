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
            int checkInNumber;
            int flightNumber;
            //  Luggage luggage;
            Luggage[] tempLuggage = new Luggage[1];//To have an object array to keep temp luggage in the mainentrance

            while (true)
            {

                //receive luggage from the hall, represented with the Luggagebuffer
                try
                {

                    Monitor.Enter(MainServer.luggageBuffer);//Locking the thread
                    if ((MainServer.luggageBuffer[0] != null) && (tempLuggage[0] == null))
                    {

                        // luggage = MainServer.luggageBuffer[0];
                        Array.Copy(MainServer.luggageBuffer, 0, tempLuggage, 0, 1);//Copy first index from luggagebuffer to the temp array

                        MainServer.luggageBuffer[0] = null;
                      //  MainServer.outPut.PrintArrivedToTheAirport(tempLuggage[0]);
                    }
                    else
                    {
                        Monitor.Wait(MainServer.luggageBuffer);//Setting the thread in waiting state
                    };
                }
                finally
                {
                    Monitor.Pulse(MainServer.luggageBuffer);//Sending signal to LuggageWorker
                    Monitor.Exit(MainServer.luggageBuffer);//Unlocking thread
                    int randomSleep = MainServer.random.Next(MainServer.randomSleepMin, MainServer.randomSleepMax);
                    Thread.Sleep(randomSleep);
                };


                //Sending the luggage to the right checkins
                try
                {

                    //Check if there is already a buffer in use for the specific flight and if thats the case, insert luggae object in that buffer
                    for (int i = 0; i < MainServer.checkInBuffers.Length; i++)//Run throught all the buffers in the array
                    {
                        checkInNumber = i;
                        Monitor.Enter(MainServer.checkInBuffers[checkInNumber]);//Locking the thread
                        if (MainServer.checkInBuffers[i].Buffer[MainServer.checkInBufferSize - 1] == null)
                        {
                            for (int j = 0; j < MainServer.checkInBuffers[i].Buffer.Length; j++)//loop through the current Buffer
                            {
                                if (tempLuggage[0] != null)
                                {
                                    if (MainServer.checkInBuffers[i].Buffer[j] != null)//If buffer index "j" in buffer "i" is not null
                                    {
                                        flightNumber = MainServer.checkInBuffers[i].Buffer[j].FlightNumber;//set the flightNumber to the one in the current buffer
                                        int luggageCount = 0;
                                        if ((j < MainServer.checkInBuffers[i].Buffer.Length - 1) && (tempLuggage[0].FlightNumber == flightNumber)) //If buffer is not full and If Luggage flightnumber is = flightNumber in the buffer
                                        {
                                            Array.Copy(tempLuggage, 0, MainServer.checkInBuffers[i].Buffer, MainServer.checkInBufferSize - 1, 1);//Copy first index from tempLuggage to the last index in the checkIn buffer array
                                            int k;
                                            for (k = 0; k < MainServer.checkInBuffers[i].Buffer.Length; k++)//Count the amount of objects in the buffer
                                            {
                                                if (MainServer.checkInBuffers[i].Buffer[k] != null)
                                                {
                                                    luggageCount++;
                                                };
                                            };
                                            if ((luggageCount > 0) && (luggageCount < MainServer.checkInBuffers[i].Buffer.Length + 1))
                                            {
                                               // MainServer.outPut.PrintCheckInBufferCapacity(checkInNumber, luggageCount);//Printing to console
                                                tempLuggage[0] = null;
                                            }
                                            else
                                            {
                                                Monitor.Wait(MainServer.checkInBuffers[checkInNumber]);//Setting the thread in waiting state
                                            };
                                        };
                                    };
                                };
                            };
                        };
                        Monitor.PulseAll(MainServer.checkInBuffers[checkInNumber]);//Sending signal to LuggageWorker
                        Monitor.Exit(MainServer.checkInBuffers[checkInNumber]);//Unlocking thread
                    };


                    //If luggage object is still not null after first check, then 
                    if (tempLuggage[0] != null)
                    {
                        try
                        {
                            for (int i = 0; i < MainServer.checkInBuffers.Length; i++)
                            {
                                checkInNumber = i;
                                Monitor.Enter(MainServer.checkInBuffers[checkInNumber]);//Locking the thread

                                int counter = 0;
                                for (int j = 0; j < MainServer.checkInBuffers[i].Buffer.Length; j++)//loop through the current Buffer
                                {
                                    if (MainServer.checkInBuffers[i].Buffer[j] != null)//Count empty spaces in buffer and check the flightNumber of the luggae in the buffer
                                    {
                                        counter++;
                                    };
                                };
                                if (counter == 0)
                                {
                                    Array.Copy(tempLuggage, 0, MainServer.checkInBuffers[i].Buffer, MainServer.checkInBufferSize - 1, 1);//Copy first index from tempLuggage to the last index in the current checkIn buffer array

                                    MainServer.outPut.PrintCheckInBufferCapacity(checkInNumber, 1);//Printing to console
                                    tempLuggage[0] = null;
                                };

                                if (tempLuggage[0] == null)
                                {
                                    i = MainServer.checkInBuffers.Length;
                                };
                                Monitor.PulseAll(MainServer.checkInBuffers[checkInNumber]);//Sending signal to LuggageWorker
                                Monitor.Exit(MainServer.checkInBuffers[checkInNumber]);//Unlocking thread
                            };

                        }
                        finally
                        {
                        };
                    };

                    //If luggage object is still not null after first and secund check, then 
                    if (tempLuggage[0] != null)
                    {
                        Monitor.Enter(MainServer.luggageBuffer);//Locking the thread
                        try
                        {
                            if (MainServer.luggageBuffer[MainServer.MaxLuggageBuffer - 1] == null)
                            {
                                Array.Copy(tempLuggage, 0, MainServer.luggageBuffer, MainServer.MaxLuggageBuffer - 1, 1);//Copy first index from tempLuggage to the last index in the luggage buffer array
                                tempLuggage[0] = null;
                            }
                            else
                            {
                                Monitor.Wait(MainServer.luggageBuffer);//Setting the thread in waiting state
                            };
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
                    int randomSleep = MainServer.random.Next(MainServer.randomSleepMin, MainServer.randomSleepMax);
                    Thread.Sleep(randomSleep);
                };

            };
        }
        #endregion
    }
}

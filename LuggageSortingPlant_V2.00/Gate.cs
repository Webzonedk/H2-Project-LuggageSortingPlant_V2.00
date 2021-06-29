using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class Gate
    {
        #region Fields
        private bool open;
        private int gateNumber;
        private Luggage[] buffer = new Luggage[MainServer.gateBufferSize];

        #endregion

        #region Properties
        public bool Open
        {
            get { return open; }
            set { open = value; }
        }

        public int GateNumber
        {
            get { return gateNumber; }
            set { gateNumber = value; }
        }
        public Luggage[] Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }


        #endregion

        #region Constructors
        public Gate()
        {

        }
        public Gate(bool open, int gateNumber)
        {
            this.open = open;
            this.gateNumber = gateNumber;
        }
        #endregion

        #region Methods

        public void Boarding()
        {
            // DateTime departure = DateTime.Now;
            int luggageCounter = 0;
            int flightNumber = -1;
            FlightPlan[] tempFlightPlan = new FlightPlan[1];
            //   FlightPlan[] tempflight = new FlightPlan[1];
            while (true)
            {
                // Monitor.Enter(MainServer.flightPlans);//Locking the thread
                Monitor.Enter(MainServer.gateBuffers[GateNumber]);//Locking the thread
                try
                {
                    if (MainServer.gateBuffers[GateNumber].Buffer[0] != null)
                    {
                        //find flight in flightplan and get departuretime for the luggage in the buffer
                        for (int i = 0; i < MainServer.flightPlans.Length; i++)
                        {
                            if (MainServer.flightPlans[i] != null && MainServer.flightPlans[i].FlightNumber == MainServer.gateBuffers[GateNumber].Buffer[0].FlightNumber)
                            {
                                // departure = MainServer.flightPlans[i].DepartureTime;//getting the depaturtime to use to open gate
                                if (((MainServer.flightPlans[i].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.gateOpenBeforeDeparture) && ((MainServer.flightPlans[i].DepartureTime - DateTime.Now).TotalSeconds >= MainServer.gateCloseBeforeDeparture))
                                {
                                    Open = true;
                                };
                                if ((MainServer.flightPlans[i].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.gateCloseBeforeDeparture)
                                {
                                    Open = false;
                                };

                                i = MainServer.flightPlans.Length - 1;
                            };
                        };


                        if (Open)// If open
                        {
                            //removing luggage from the gate buffer
                            //if (MainServer.gateBuffers[gateNumber].Buffer[0] != null)
                            //{
                            Array.Copy(MainServer.gateBuffers[GateNumber].Buffer, 0, Buffer, Buffer.Length - 1, 1);//Copy first index from gate buffer to the temp array

                            // MainServer.outPut.PrintGateCapacity(GateNumber, luggageCounter);
                            MainServer.gateBuffers[GateNumber].Buffer[0] = null;
                            //};
                        };
                    }
                    else
                    {
                        //   Monitor.Wait(MainServer.flightPlans);//Locking the thread
                        Monitor.Wait(MainServer.gateBuffers[GateNumber]);//Locking the thread
                    }
                }
                finally
                {
                    Monitor.PulseAll(MainServer.gateBuffers[GateNumber]);//Sending signal to other thread
                    Monitor.Exit(MainServer.gateBuffers[GateNumber]);//Release the lock
                    //Monitor.PulseAll(MainServer.flightPlans);//Sending signal to other thread
                    //Monitor.Exit(MainServer.flightPlans);//Release the lock

                    //int randomSleep = MainServer.random.Next(MainServer.randomSleepMin, MainServer.randomSleepMax);
                    //Thread.Sleep(randomSleep);
                };




                //------------------------------------------------------------------------
                //Sorting the  boarding buffer in the gate
                //------------------------------------------------------------------------

                // Monitor.Enter(MainServer.gates[GateNumber].Buffer);//Locking the thread
                try
                {
                    for (int i = 0; i < MainServer.gates[GateNumber].Buffer.Length - 1; i++)
                    {
                        if (MainServer.gates[GateNumber].Buffer[i] == null)
                        {
                            MainServer.gates[GateNumber].Buffer[i] = MainServer.gates[GateNumber].Buffer[i + 1];
                            MainServer.gates[GateNumber].Buffer[i + 1] = null;
                        }
                    }
                }
                finally
                {
                    //Monitor.PulseAll(MainServer.gates[GateNumber].Buffer);//Sending signal to other thread
                    //Monitor.Exit(MainServer.gates[GateNumber].Buffer);//Release the lock
                }


                //------------------------------------------------------------------------
                //TakeOff and emptuing the gate Boarding buffer
                //------------------------------------------------------------------------
                //find flight in flightplan and get departuretime for the luggage in the buffer
                if (MainServer.gates[GateNumber].Buffer[0] != null)
                {
                    for (int i = 0; i < MainServer.flightPlans.Length; i++)
                    {
                        if (MainServer.flightPlans[i] != null && MainServer.flightPlans[i].FlightNumber == MainServer.gates[GateNumber].Buffer[0].FlightNumber)
                        {
                            flightNumber = MainServer.gates[GateNumber].Buffer[0].FlightNumber;

                            if ((MainServer.flightPlans[i].DepartureTime - DateTime.Now).TotalSeconds <= MainServer.gateCloseBeforeDeparture)
                            {
                                Open = false;
                                MainServer.gates[GateNumber].Buffer[0] = null;
                                luggageCounter++;
                                i = MainServer.flightPlans.Length - 1;
                            };
                        };
                    };
                };

               
                Thread.Sleep(1);
            };
        }
        #endregion
    }
}

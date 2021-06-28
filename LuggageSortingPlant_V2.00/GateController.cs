using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class GateController
    {
        public EventHandler openCloseGates;
        private Thread gateOpenCloseThread;
        private int gateNumber;

        public int GateNumber
        {
            get { return gateNumber; }
            set { gateNumber = value; }
        }


        public Thread GateOpenCloseThread
        {
            get { return gateOpenCloseThread; }
            set { gateOpenCloseThread = value; }
        }



        public GateController(int gateNumber)
        {
            this.gateNumber = gateNumber;
            Thread gateColorThread = new Thread(Run);
            gateColorThread.Start();
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    if (MainServer.gates[GateNumber] != null)
                    {
                        bool status = MainServer.gates[GateNumber].Open;
                        openCloseGates?.Invoke(this, new GateEvent(GateNumber, status));//Invoking the luggage and send it to the listener
                    }
                    Thread.Sleep(50);
                }
                finally
                {

                }
            }
        }
    }
}

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
                        int counter = CountLuggageInGate();
                        bool status = MainServer.gates[GateNumber].Open;
                        openCloseGates?.Invoke(this, new GateEvent(GateNumber, status, counter));//Invoking the luggage and send it to the listener
                    };
                }
                finally
                {

                };
                Thread.Sleep(1);
            };
        }

        private int CountLuggageInGate()
        {
            try
            {
                int count = 0;
                for (int i = 0; i < MainServer.gates[GateNumber].Buffer.Length; i++)
                {
                    if (MainServer.gates[GateNumber].Buffer[i] != null)
                    {
                        count++;
                    };
                };
                return count;
            }
            finally
            {
            };

        }
    }
}

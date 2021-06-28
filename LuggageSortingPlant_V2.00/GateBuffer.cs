using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    class GateBuffer
    {
        #region Fields
        private int gateNumber;


        private Luggage[] buffer = new Luggage[MainServer.gateBufferSize];

        #endregion



        #region Properties
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
        public GateBuffer()
        {

        }
        //Initializing
        public GateBuffer(int gateNumber)
        {
            this.gateNumber = gateNumber;
        }
        #endregion

        #region Methods
        public void ReorderingGateBuffer()//Not yet adjusted to fit gatebuffer
        {
            while (true)
            {
                //if (MainServer.gateBuffers[gateNumber].Buffer[MainServer.gateBuffers[gateNumber].Buffer.Length - 1] !=null)
                //{
                Monitor.Enter(MainServer.gateBuffers[gateNumber]);//Locking the thread
                try
                {
                    for (int i = 0; i < MainServer.gateBuffers[gateNumber].Buffer.Length - 1; i++)
                    {
                        if (MainServer.gateBuffers[gateNumber].Buffer[i] == null)
                        {
                            MainServer.gateBuffers[gateNumber].Buffer[i] = MainServer.gateBuffers[gateNumber].Buffer[i + 1];
                            MainServer.gateBuffers[gateNumber].Buffer[i + 1] = null;
                        }
                    }
                }
                finally
                {
                    Monitor.PulseAll(MainServer.gateBuffers[gateNumber]);//Sending signal to other thread
                    Monitor.Exit(MainServer.gateBuffers[gateNumber]);//Release the lock
                }

                //}
                Thread.Sleep(1);
            }
        }
        #endregion
    }
}


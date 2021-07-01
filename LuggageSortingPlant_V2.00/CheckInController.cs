using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    //--------------------------------------------------------------
    //This is a controller class to get the current state of each
    //checkin counter to be able to show colors in the xaml
    //--------------------------------------------------------------
    class CheckInController
    {

        public EventHandler openCloseCheckIns;
        private int checkInNumber;

        public int CheckInNumber
        {
            get { return checkInNumber; }
            set { checkInNumber = value; }
        }

        public CheckInController(int checkInNumber)
        {
            this.checkInNumber = checkInNumber;
            Thread checkInColorThread = new Thread(Run);
            checkInColorThread.Start();
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    if (MainServer.checkIns[CheckInNumber] != null)
                    {
                        bool status = MainServer.checkIns[CheckInNumber].Open;
                        openCloseCheckIns?.Invoke(this, new CheckInEvent(CheckInNumber, status));//Invoking the luggage and send it to the listener
                    };
                }
                finally
                {
                };
                Thread.Sleep(1);
            };
        }
    }
}

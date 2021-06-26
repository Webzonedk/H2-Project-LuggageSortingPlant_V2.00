using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading
    ;

namespace LuggageSortingPlant_V2._00
{
    //--------------------------------------------------------------
    //This is a controller class to get the current state of each
    //checkin counter to be able to show colors in the xaml
    //--------------------------------------------------------------
    class CheckInController
    {

        public EventHandler OpenCloseCheckIns;
        private int checkInNumber;
        private Thread checkInOpenCloseThread;

        public int CheckInNumber
        {
            get { return checkInNumber; }
            set { checkInNumber = value; }
        }


        public Thread CheckInOpenCloseThread
        {
            get { return checkInOpenCloseThread; }
            set { checkInOpenCloseThread = value; }
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
                CheckIn checkIn = MainServer.checkIns[checkInNumber];
                OpenCloseCheckIns?.Invoke(this, new CheckInEvent(checkIn));//Invoking the luggage and send it to the listener
            }
        }

        //private CheckIn GetCheckInState()
        //{
        //    for (int i = 0; i < MainServer.checkIns.Length; i++)
        //    {

        //    }
        //}
    }
}

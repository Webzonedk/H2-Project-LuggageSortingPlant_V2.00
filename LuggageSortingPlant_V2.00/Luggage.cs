using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    public class Luggage
    {
        #region Fields
        private int luggageNumber;
        private int passengerNumber;
        private string passengerName;
        private int flightNumber;
        private DateTime checkInTimeStamp;
        private DateTime sortInTimeStmap;
        private DateTime sortOutTimeStamp;
        private DateTime gateArrivalTimeStamp;


        #endregion



        #region Properties
        public int LuggageNumber
        {
            get { return luggageNumber; }
            set { luggageNumber = value; }
        }
        public int PassengerNumber
        {
            get { return passengerNumber; }
            set { passengerNumber = value; }
        }
        public string PassengerName
        {
            get { return passengerName; }
            set { passengerName = value; }
        }
        public int FlightNumber
        {
            get { return flightNumber; }
            set { flightNumber = value; }
        }
        public DateTime CheckInTimeStamp
        {
            get { return checkInTimeStamp; }
            set { checkInTimeStamp = value; }
        }

        public DateTime SortInTimeStmap
        {
            get { return sortInTimeStmap; }
            set { sortInTimeStmap = value; }
        }

        public DateTime SortOutTimeStamp
        {
            get { return sortOutTimeStamp; }
            set { sortOutTimeStamp = value; }
        }

        public DateTime GateArrivalTimeStamp
        {
            get { return gateArrivalTimeStamp; }
            set { gateArrivalTimeStamp = value; }
        }


        #endregion

        #region Constructors
        public Luggage()
        {

        }
        public Luggage(int luggageNumber, int passengerNumber, string passengerName, int flightNumber, DateTime checkInTimeStamp, DateTime sortInTimeStmap, DateTime sortOutTimeStamp, DateTime gateArrivalTimeStamp)
        {
            this.luggageNumber = luggageNumber;
            this.passengerNumber = passengerNumber;
            this.passengerName = passengerName;
            this.flightNumber = flightNumber;
            this.checkInTimeStamp = checkInTimeStamp;
            this.sortInTimeStmap = sortInTimeStmap;
            this.sortOutTimeStamp = sortOutTimeStamp;
            this.gateArrivalTimeStamp = gateArrivalTimeStamp;
        }



        #endregion

        #region Methods
        #endregion
    }
}

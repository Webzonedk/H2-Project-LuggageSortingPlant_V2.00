using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LuggageSortingPlant_V2._00
{
    public class FlightPlan
    {
        #region Fields
        private int flightNumber;
        private string destination;
        private int seats;
        private int gateNumber;
        private DateTime departureTime;




        #endregion

        #region Properties


        public int FlightNumber
        {
            get { return flightNumber; }
            set { flightNumber = value; }
        }
        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public int Seats
        {
            get { return seats; }
            set { seats = value; }
        }
        public int GateNumber
        {
            get { return gateNumber; }
            set { gateNumber = value; }
        }

        public DateTime DepartureTime
        {
            get { return departureTime; }
            set { departureTime = value; }
        }




        #endregion

        #region Constructors
        public FlightPlan()
        {

        }
        //Initializing
        public FlightPlan(int flightNumber, string destination, int seats, int gateNumber, DateTime departureTime)
        {
            this.flightNumber = flightNumber;
            this.destination = destination;
            this.seats = seats;
            this.gateNumber = gateNumber;
            this.departureTime = departureTime;
        }


        #endregion

        #region Methods

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//-------------------------------------------------------------------------------------
// This class is no longer in use. It was used to print to console before migrating to WPF
//-------------------------------------------------------------------------------------

namespace LuggageSortingPlant_V2._00
{
    class OutPut
    {

        public void PrintFlightPlan(int i)//Method with argument
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Fligtnumber {MainServer.flightPlans[i].FlightNumber} " +
                $"with destination {MainServer.flightPlans[i].Destination} " +
                $"with {MainServer.flightPlans[i].Seats} " +
                $"departure from gate: {MainServer.flightPlans[i].GateNumber} " +
                $"at: {MainServer.flightPlans[i].DepartureTime.ToLongTimeString()}");
            Console.ResetColor();
        }

        public void PrintLuggage(int i)//Method with argument
        {
            Console.ResetColor();
            Console.WriteLine($"Luggagge nr.: {MainServer.luggageBuffer[i].LuggageNumber} " +
                $"passenger nr.:{MainServer.luggageBuffer[i].PassengerNumber} " +
                $"Name: {MainServer.luggageBuffer[i].PassengerName}\t\t" +
                $"For flight: {MainServer.luggageBuffer[i].FlightNumber} has been created.");
            Console.ResetColor();
        }

        public void PrintArrivedToTheAirport(Luggage luggage) //MainEntrance Arrival
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Passenger: {luggage.PassengerName}, " +
                $"with Passenger number: {luggage.PassengerNumber} " +
                $"and luggage number: {luggage.LuggageNumber} " +
                $"for flight number: {luggage.FlightNumber}, has arrived to the airport. ");
            Console.ResetColor();
        }

        public void PrintCheckInBufferCapacity(int checkInNumber, int luggageInBuffer)//Printing the capacity of the checkIn buffer
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"CheckIn conveyor belt: {checkInNumber} now have {luggageInBuffer} / {MainServer.checkInBufferSize} Suitcases on the band ");
            Console.ResetColor();
        }

        public void PrintCheckInBufferWorkerOutput(int checkInNumber)//Not active at the moment
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Reordered CheckIn buffer {checkInNumber}");
            Console.ResetColor();
        }

        public void PrintCheckInArrival(Luggage luggage) //Printing when luggage arrives to checkin
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Luggage number: {luggage.LuggageNumber} " +
                $"for flight number: {luggage.FlightNumber}, has checked in " +
                $"at: {luggage.CheckInTimeStamp.ToLongTimeString()}.");
            Console.ResetColor();
        }

        public void PrintCheckInOut(Luggage luggage)//Not active at the moment. but can be added for debugging
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.ResetColor();
        }

        public void PrintSortingBufferCapacity(int luggageInBuffer)//Printing the capacity of the sorting buffer
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Conveyor belt for the sorting unit now have {luggageInBuffer} / {MainServer.sortBufferSize} Suitcases on the band ");
            Console.ResetColor();
        }

        public void PrintSortingArrival(Luggage luggage)//Not finish
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Luggage number: {luggage.LuggageNumber} " +
                $"for flight number: {luggage.FlightNumber}," +
                $" has arrived to the sortingUnit at {luggage.SortInTimeStmap.ToLongTimeString()}");
            Console.ResetColor();
        }

        public void PrintSortingDeparting(Luggage luggage)//Not finish
        {
            Console.ResetColor();
            Console.ForegroundColor  = ConsoleColor.Green;
            Console.WriteLine($"Luggage number: {luggage.LuggageNumber} " +
                $"for flight number: {luggage.FlightNumber}," +
                $" has left the the sortingUnit at {luggage.SortOutTimeStamp.ToLongTimeString()}");
            Console.ResetColor();
        }
        public void PrintSortedToGate(Luggage luggage, int gateNumber)//Not finish
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Luggage number: {luggage.LuggageNumber} " +
                $"for flight number: {luggage.FlightNumber}," +
                $" has left the the sortingUnit at {luggage.SortOutTimeStamp.ToLongTimeString()}" +
                $"and is now in queue at gate: {gateNumber} ");
            Console.ResetColor();
        }

        public void PrintLuggageReturnedToSortingBuffer(Luggage luggage)//Not finish
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Luggage number: {luggage.LuggageNumber} " +
                $"for flight number: {luggage.FlightNumber}," +
                $" has been relocated to next flight to same destination");
            Console.ResetColor();
        }
        public void PrintGateCapacity(int gateNumber, int luggageInBuffer)//Printing the capacity of the checkIn buffer
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"The plane at Gate: {gateNumber} now have {luggageInBuffer} / {MainServer.checkInBufferSize} Suitcases on board ");
            Console.ResetColor();
        }

        public void PrintTakeOff(int gateNumber, int flightNumber, int luggageInBuffer)//Printing the capacity of the checkIn buffer
        {
            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"The plane {flightNumber} at Gate: {gateNumber} took of with {luggageInBuffer} Suitcases on board ");
            Console.ResetColor();
        }


    }
}

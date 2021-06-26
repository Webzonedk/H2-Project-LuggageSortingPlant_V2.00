using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuggageSortingPlant_V2._00
{
    class CheckInEvent : EventArgs
    {
        public int CheckInNumber { get; private set; }
        public bool Status { get; private set; }

        public CheckInEvent(int checkInNumber, bool status)
        {
            CheckInNumber = checkInNumber;
            Status = status;
        }
    }
}

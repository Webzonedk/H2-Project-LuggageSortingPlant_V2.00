using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuggageSortingPlant_V2._00
{
    class CheckInBufferEvent : EventArgs
    {
        public int CheckInNumber { get; private set; }
        public int Count { get;  set; }

        public CheckInBufferEvent(int checkInNumber, int count)
        {
            CheckInNumber = checkInNumber;
            Count = count;
        }
    }
}

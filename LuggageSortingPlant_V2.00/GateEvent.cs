using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuggageSortingPlant_V2._00
{
    class GateEvent : EventArgs
    {
        public int GateNumber { get; private set; }
        public bool Status { get; private set; }
        public int Count { get; private set; }

        public GateEvent(int gateNumber, bool status, int count)
        {
            GateNumber = gateNumber;
            Status = status;
            Count = count;
        }
    }
}

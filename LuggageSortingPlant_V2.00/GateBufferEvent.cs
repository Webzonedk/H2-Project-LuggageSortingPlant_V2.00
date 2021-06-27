using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuggageSortingPlant_V2._00
{
    class GateBufferEvent : EventArgs
    {
        public int GateNumber { get; set; }
        public int Count { get; set; }

        public GateBufferEvent(int gateNumber, int count)
        {
            GateNumber = gateNumber;
            Count = count;
        }
    }
}

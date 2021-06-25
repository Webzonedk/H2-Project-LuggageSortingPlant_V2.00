using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuggageSortingPlant_V2._00
{
    class CheckInEvent : EventArgs
    {
        public int Count { get; set; }

        public CheckInEvent(int count)
        {
            Count = count;
        }
    }
}

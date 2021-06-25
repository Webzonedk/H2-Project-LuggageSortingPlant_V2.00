using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuggageSortingPlant_V2._00
{
    class CheckInEvent : EventArgs
    {
        public CheckIn CheckIn { get; set; }

        public CheckInEvent(CheckIn checkIn)
        {
            CheckIn = checkIn;
        }
    }
}

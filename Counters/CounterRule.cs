using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringService.Counters
{
    class CounterRule
    {
        public int CounterId { get; set; }
        public double CriticalValue { get; set; }
        public bool IsAbove { get; set; }
    }
}

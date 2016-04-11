using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringService.Counters
{
    class Rule
    {
        public List<CounterRule> counterRules { get; }

        public Rule()
        {
            counterRules = new List<CounterRule>();
        }
    }
}

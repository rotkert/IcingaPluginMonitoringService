using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringService.Communication
{
    enum MessageType
    {
        Counters,
        Report,
        Configuration
    }

    class QueueElement
    {
        public MessageType messageType { get; set; }
        public String arg1 { get; set; }
    }
}

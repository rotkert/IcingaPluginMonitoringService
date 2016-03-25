using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringService
{
    public partial class MonitoringService : ServiceBase
    {
        private PerfCountersHandler perfCountersHandler;

        public MonitoringService()
        {
            InitializeComponent();
            initializeEventLog();

            perfCountersHandler = new PerfCountersHandler();
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Monitoring service started", EventLogEntryType.Information);
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 10000; // 10 seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Monitoring service stopped", EventLogEntryType.Information);
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            perfCountersHandler.checkCounters();
        }

        private void initializeEventLog()
        {
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("MonitoringServiceSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource("MonitoringServiceSource", "MonitoringServiceLog");
            }
            eventLog1.Source = "MonitoringServiceSource";
            eventLog1.Log = "MonitoringServiceLog";
        }
    }
}

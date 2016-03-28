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
        private DataSender dataSender;

        public MonitoringService()
        {
            InitializeComponent();
            initializeEventLog();

            perfCountersHandler = new PerfCountersHandler();
            dataSender = new DataSender();
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Monitoring service started", EventLogEntryType.Information);

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 1 minute
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            System.Timers.Timer sendingTimer = new System.Timers.Timer();
            sendingTimer.Interval = 120000; // 2 minutes
            sendingTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnSendingTimer);
            sendingTimer.Start();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Monitoring service stopped", EventLogEntryType.Information);
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            perfCountersHandler.checkCounters();
        }

        public void OnSendingTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            dataSender.send();
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

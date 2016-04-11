using MonitoringService.Communication;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonitoringService
{
    public partial class MonitoringService : ServiceBase
    {
        private BlockingCollection<QueueElement> blockingQueue;
        private PerfCountersHandler perfCountersHandler;
        private DataSender dataSender;

        public MonitoringService()
        {
            InitializeComponent();
            initializeEventLog();

            blockingQueue = new BlockingCollection<QueueElement>();
            perfCountersHandler = new PerfCountersHandler(blockingQueue);
            dataSender = new DataSender();

            Thread senderThread = new Thread(() => dataSender.work(blockingQueue));
            senderThread.Start();
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Monitoring service started", EventLogEntryType.Information);

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 30000; // 5 sec
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

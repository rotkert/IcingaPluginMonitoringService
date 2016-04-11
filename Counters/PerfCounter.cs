using System;
using System.Collections.Generic;
using System.Linq;

namespace MonitoringService
{
    class PerfCounter
    {
        private System.Diagnostics.PerformanceCounter counter;
        private String serviceName;

        private int maxChecks;
        private int minChecks;
        private List<CheckResult> checkResults;

        private bool critical;

        public PerfCounter(String category, String instance, String counter, String serviceName, int maxChecks, int minChecks)
        {
            this.counter = new System.Diagnostics.PerformanceCounter();

            ((System.ComponentModel.ISupportInitialize)(this.counter)).BeginInit();
            this.counter.CategoryName = category;
            this.counter.CounterName = counter;
            this.counter.InstanceName = instance;
            ((System.ComponentModel.ISupportInitialize)(this.counter)).EndInit();

            this.serviceName = serviceName;
            this.maxChecks = maxChecks;
            this.minChecks = minChecks;

            checkResults = new List<CheckResult>();
            for (int i = 0; i < minChecks; i++)
            {
                processCheck(0, 0);
            }   

            critical = false;
        }

        public void check()
        {
            long timestamp = getTimestamp();
            double value = counter.NextValue();
            processCheck(value, timestamp);
        }

        public double getAvgForPeriod(int period)
        {
            var avgItems = checkResults.Skip(checkResults.Count - period);
            double avg = avgItems.Average(x => x.Result);
            return avg;
        }

        public CheckResult getCheckForMaxPeriod()
        {
            CheckResult first = checkResults[minChecks];
            CheckResult last = checkResults.Last();

            return (new CheckResult
            {
                ServiceName = serviceName,
                Result = (first.Result + last.Result) / 2,
                Timestamp = (first.Timestamp + last.Timestamp) /2 
            });
        }

        public IEnumerable<CheckResult> getAllChecks()
        {
            return checkResults.Skip(minChecks);
        }

        public void clear()
        {
            checkResults = checkResults.Skip(checkResults.Count - minChecks).ToList();
        }

        private void processCheck(double value, long timestamp)
        {
            checkResults.Add(new CheckResult
            {
                ServiceName = serviceName,
                Result = value,
                Timestamp = timestamp
            });
        }

        private long getTimestamp()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((DateTime.Now.ToUniversalTime() - epoch).TotalSeconds);
        }
    }
}

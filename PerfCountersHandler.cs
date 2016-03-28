using MonitoringService.DataSource;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringService
{
    class PerfCountersHandler
    {
        private List<PerfCounter> counters;

        public PerfCountersHandler()
        {
            counters = loadCounters();
        }

        public void checkCounters()
        {
            using (var dbContext = new MonitoringServiceDbContext())
            {
                dbContext.Database.EnsureCreated();
                foreach (PerfCounter counter in counters)
                {
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var unixDateTime = Convert.ToInt64((DateTime.Now.ToUniversalTime() - epoch).TotalSeconds);

                    bool isCritical = counter.check();
                    dbContext.CheckResults.Add(new CheckResult
                    {
                        ServiceName = counter.getServiceName(),
                        Result = counter.getLastValue(),
                        Timestamp = unixDateTime,
                        Crtitical = counter.getCritivalValue(),
                        HasExceeded = isCritical
                    });
                    dbContext.SaveChanges();
                }
            }
        }

        private List<PerfCounter> loadCounters()
        {
            List<PerfCounter> perfCounters = new List<PerfCounter>();
            perfCounters.Add(new PerfCounter("Procesor","_Total", "Czas procesora (%)", "diagnostics", 2, 10, false));
            return perfCounters;
        }

    }
}

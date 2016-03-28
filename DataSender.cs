using Microsoft.Data.Entity;
using MonitoringService.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringService
{
    class DataSender
    {
        public void send()
        {
            sendChecks();
        }

        private void sendChecks()
        {
            List<IcingaWebServiceRef.Check> checks = new List<IcingaWebServiceRef.Check>();
            using (var dbContext = new MonitoringServiceDbContext())
            {
                dbContext.Database.EnsureCreated();
                DbSet<CheckResult> currentResults = dbContext.Set<CheckResult>();
                foreach (CheckResult checkResult in currentResults)
                {
                    checks.Add(new IcingaWebServiceRef.Check
                    {
                        timestamp = checkResult.Timestamp,
                        hostName = "windows7",
                        serviceName = checkResult.ServiceName,
                        state = "0",
                        result = checkResult.Result,
                        hasExceeded = checkResult.HasExceeded
                    });
                }

                IcingaWebServiceRef.IcingaWebServiceClient client = new IcingaWebServiceRef.IcingaWebServiceClient();
                string result = client.processChecks(checks.ToArray());

                dbContext.RemoveRange(currentResults);
                dbContext.SaveChanges();
            }
        }
    }
}

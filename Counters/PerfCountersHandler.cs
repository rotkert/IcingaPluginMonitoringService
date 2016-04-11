using IcingaWebServiceRef;
using MonitoringService.Communication;
using MonitoringService.Counters;
using MonitoringService.DataSource;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MonitoringService
{
    class PerfCountersHandler
    {
        private BlockingCollection<QueueElement> blockingQueue; 
        private List<PerfCounter> counters;
        private List<Rule> rules;

        public PerfCountersHandler(BlockingCollection<QueueElement> blockingQueue)
        {
            this.blockingQueue = blockingQueue;
            counters = new List<PerfCounter>();
            rules = new List<Rule>();
            loadCounters();
        }

        public void checkCounters()
        {
            foreach (PerfCounter counter in counters)
            {
                counter.check();
            }

            foreach (Rule rule in rules)
            {
                PerfCounter pc = null;
                bool isCritical = true;
                foreach (CounterRule counterRule in rule.counterRules)
                {
                    pc = counters[counterRule.CounterId];
                    if (counters[counterRule.CounterId].getAvgForPeriod(2) < counterRule.CriticalValue)
                    {
                        isCritical = false;
                    }
                }

                if (isCritical)
                {
                    using (var dbContext = new MonitoringServiceDbContext())
                    {
                        dbContext.Database.EnsureCreated();
                        handleCritical(pc, dbContext);
                    }
                }
            }
        }

        private void handleCritical(PerfCounter counter, MonitoringServiceDbContext dbContext)
        {
            startPerfMon();
            saveAllValues(counter, dbContext);
            callDataSender();
            counter.clear();
        }

        private void startPerfMon()
        {

        }

        private void saveAllValues(PerfCounter counter, MonitoringServiceDbContext dbContext)
        {
            foreach(CheckResult checkResult in counter.getAllChecks())
            {
                dbContext.CheckResults.Add(checkResult);
            }

            dbContext.SaveChanges();
        }

        private void callDataSender()
        {
            blockingQueue.Add(new QueueElement
            {
                messageType = MessageType.Counters
            });
        }

        private void handlePeriodEnd(PerfCounter counter, MonitoringServiceDbContext dbContext)
        {
            saveAvg(counter, dbContext);
            callDataSender();
            counter.clear();
        }

        private void saveAvg(PerfCounter counter, MonitoringServiceDbContext dbContext)
        {
            dbContext.Add(counter.getCheckForMaxPeriod());
            dbContext.SaveChanges();
        }

        private void loadCounters()
        {
            IcingaWebServiceClient client = new IcingaWebServiceClient();
            cfgRulesWrapper crw  = client.getConfig();

            foreach (CfgCounterDetails cd in crw.cfgCounters)
            {
                counters.Add(new PerfCounter(cd.category, cd.instance, cd.counter, cd.serviceName, cd.maxChecks, cd.minChecks));
            }
            
            foreach (CfgRule cfgRule in crw.cfgRules)
            {
                Rule newRule = new Rule();
                foreach (CfgCounterRule cfgCounterRule in cfgRule.cfgCounterRulesWrapper)
                {
                    newRule.counterRules.Add(new CounterRule
                    {
                        CounterId = cfgCounterRule.counterId,
                        CriticalValue = cfgCounterRule.criticalValue,
                        IsAbove = cfgCounterRule.above
                    });
                }

                rules.Add(newRule);
            }
        }

    }
}

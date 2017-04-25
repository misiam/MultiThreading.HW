using IANA.ZoneServer.Requests;
using MultiThreading.HW.IANA.ZoneServer.Definitions;
using MultiThreading.HW.IANA.ZoneServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.HW.IANA.Threads
{
    public class ThreadsClient
    {
        private IanaDbClient client = new IanaDbClient();
        private object _lockIterator = new object();
        public IList<IanaDomain> GetDomains(string url)
        {
            var logger = Logger.Instance;
            Func<IList<IanaDomain>> func = () =>
            {
                var domains = new List<IanaDomain>();
                var thread = new Thread(() =>
                {
                    domains.AddRange(client.GetDomains(url));
                });
                thread.Start();
                thread.Join();
                
                return domains;

            };

            return logger.WrapWithTimeLog(func, "GetDomains_Threads");
        }

        public IList<IanaDomainWIthWhois> GetWhois(IList<IanaDomain> domains)
        {
            var logger = Logger.Instance;
            int i = 0;
            var whoisList = new List<IanaDomainWIthWhois>();

            ThreadStart threadJob = () => {
                string threadName = Thread.CurrentThread.Name;
                do
                {
                    int local_i;
                    lock (this._lockIterator)
                    {
                        local_i = i++;
                    }
                    var ianaWhois = client.GetWhoisAddress(domains[local_i]);
                    lock (client)
                    {
                        whoisList.Add(ianaWhois);
                    }
                    logger.Log($"{threadName}: [{local_i}] {ianaWhois.Name} {ianaWhois.Whois}");
                } while (i < domains.Count);
            };

            var threadList = Enumerable.Range(1, Environment.ProcessorCount*5).Select(x => new Thread(threadJob) { Name = $"thread_{x}" }).ToList();
            Func<IList<IanaDomainWIthWhois>> func = () =>
            {
                threadList.ForEach(t => t.Start());
                threadList.ForEach(t => t.Join());

                return whoisList;
            };
            return logger.WrapWithTimeLog(func, "GetWhois_Threads");
        }
    }
}

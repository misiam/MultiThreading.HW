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
    public class ThreadPoolClient 
    {
        private IanaDbClient client = new IanaDbClient();

        public IList<IanaDomain> GetDomains(string url)
        {
            var logger = Logger.Instance;
            Func<IList<IanaDomain>> func = () =>
            {
                var resetGetDomains = new ManualResetEvent(false);

                var domains = new List<IanaDomain>();
                ThreadPool.QueueUserWorkItem((state) => {
                    domains.AddRange(client.GetDomains(url));
                    resetGetDomains.Set();
                });

                while (!resetGetDomains.WaitOne(TimeSpan.FromSeconds(1)))
                {
                    logger.Log($"Continue GetDomains_Threads");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                return domains;
            };
            return logger.WrapWithTimeLog(func, "GetDomains_ThreadPool");
        }

        public IList<IanaDomainWIthWhois> GetWhois(IList<IanaDomain> domains)
        {
            var logger = Logger.Instance;

            Func<IList<IanaDomainWIthWhois>> func = () => {
                ManualResetEvent resetGetWhois = new ManualResetEvent(false);

                var whoisList = new List<IanaDomainWIthWhois>();
                int i = 0;
                foreach (var item in domains)
                {
                    ThreadPool.QueueUserWorkItem((state) => {
                        var domain = (IanaDomain)state;
                        var ianaWhois = client.GetWhoisAddress(domain);
                        lock (client)
                        {
                            whoisList.Add(ianaWhois);
                            if (whoisList.Count == domains.Count)
                            {
                                resetGetWhois.Set();
                            }
                        }
                    }, item);
                }

                while (!resetGetWhois.WaitOne(TimeSpan.FromSeconds(2)))
                {
                    logger.Log($"Continue GetWhois..{whoisList.Count} of {domains.Count}");
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }

                return whoisList;
            };
            return logger.WrapWithTimeLog(func, "GetWhois_ThreadPool");
        }




        private class RefObject<T> 
        {
            public string Url { get; set; }
            public T Item { get; set; }
        }
    }
}

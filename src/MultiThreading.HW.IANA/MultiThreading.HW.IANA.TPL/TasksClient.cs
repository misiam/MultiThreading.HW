using IANA.ZoneServer.Requests;
using MultiThreading.HW.IANA.ZoneServer.Definitions;
using MultiThreading.HW.IANA.ZoneServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading.HW.IANA.TPL
{
    public class TasksClient
    {
        private IanaDbClient client = new IanaDbClient();
        public Task<IList<IanaDomain>> GetDomains(string url, bool startTask = false)
        {
            var task = new Task<IList<IanaDomain>>(() => client.GetDomains(url));
            if (startTask)
            {
                task.Start();
            }
            return task;
        }

        public IList<IanaDomain> GetDomainsFromTask(string url)
        {
            var logger = Logger.Instance;

            var task = this.GetDomains(url);
            Func<IList<IanaDomain>> func = () =>
            {
                task.Start();
                task.Wait();
                return task.Result;
            };

            return logger.WrapWithTimeLog(func, "GetDomainsFromTask");
        }
        public Task<IList<IanaDomainWIthWhois>> GetWhois(IList<IanaDomain> domains, bool startTask = false)
        {
            var logger = Logger.Instance;
            var task = new Task<IList<IanaDomainWIthWhois>>(
                () =>
                {
                    Func<IList<IanaDomainWIthWhois>> func = () =>
                    {
                        return domains.AsParallel().Select(d =>
                        {
                            var whois = client.GetWhoisAddress(d);
                            logger.Log($"{whois.Name} : {whois.Whois}");
                            return whois;
                        }).ToList();
                    };

                    return logger.WrapWithTimeLog(func, "GetWhois parallel");
                }
                );
            if (startTask)
            {
                task.Start();
            }
            return task;
        }

        public void GetWhoisFromTask(IList<IanaDomain> domains)
        {
            var getWhoisTask = GetWhois(domains);
            var logger = Logger.Instance;
            getWhoisTask.Start();
        }
    }
}

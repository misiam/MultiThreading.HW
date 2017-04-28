using IANA.ZoneServer.Requests;
using MultiThreading.HW.IANA.ZoneServer.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading.HW.IANA.TPL
{
    public class AsyncTasksClient
    {
        private TasksClient client = new TasksClient();
        public async Task<IList<IanaDomain>> GetDomains(string url, bool startTask = false)
        {
            var getDomainsTask = client.GetDomains(url);
            return await getDomainsTask;
        }

        public async Task<IList<IanaDomainWIthWhois>> GetWhois(IList<IanaDomain> domains)
        {
            return await client.GetWhois(domains);
        }
    }
}

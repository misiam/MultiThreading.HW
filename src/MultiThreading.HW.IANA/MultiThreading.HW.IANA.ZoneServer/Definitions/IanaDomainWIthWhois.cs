using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading.HW.IANA.ZoneServer.Definitions
{
    public class IanaDomainWIthWhois :IanaDomain
    {
        public IanaDomainWIthWhois()
        {
        }

        public IanaDomainWIthWhois(IanaDomain ianaDomain) : this()
        {
            this.Name = ianaDomain.Name;
            this.Href = ianaDomain.Href;
        }

        public IanaDomainWIthWhois(IanaDomain ianaDomain, string whois) : this(ianaDomain)
        {
            this.Whois = whois;
        }

        public string Whois { get; set; }
    }
}

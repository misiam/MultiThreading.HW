using MultiThreading.HW.IANA.ZoneServer.Definitions;
using MultiThreading.HW.IANA.ZoneServer.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace   IANA.ZoneServer.Requests
{
    public class IanaDbClient : HttpClient
    {
        public const string IANA_ROOT = "https://www.iana.org";
        public const string IANA_ROOT_DB = IANA_ROOT + "/domains/root/db";


        public IList<IanaDomain> GetDomains(string url = IANA_ROOT_DB)
        {
            const string XPATH_DB_LIST = "//span[@class='domain tld']/a";
            const string ATTRIBUTE_NAME = "href";

            var nodeCollection = this.GetLinesInner(url, XPATH_DB_LIST);
            var node = nodeCollection.Select(x=> new IanaDomain {
                    Name = x.InnerText,
                    Href = IANA_ROOT + x.Attributes.First(att=>att.Name== ATTRIBUTE_NAME).Value,
                });

            return node.ToList();
        }

        public IanaDomainWIthWhois GetWhoisAddress(IanaDomain ianaDomain)
        {
            const string XPATH_WHOIS = "//div[@id='main_right']/p/b";
            var nodeCollection = this.GetLinesInner( ianaDomain.Href, XPATH_WHOIS);

            if(nodeCollection != null && nodeCollection.Count > 0)
            {
                var whoisLabel = nodeCollection.Last();
                return new IanaDomainWIthWhois(ianaDomain, GetWhoisValueOrEmpty(whoisLabel));
            }
            else
            {
                return new IanaDomainWIthWhois(ianaDomain, string.Empty);
            }
        }

        private string GetWhoisValueOrEmpty(HtmlAgilityPack.HtmlNode node)
        {
            bool nodeHasWhoIs = node.ParentNode.InnerText.IndexOf("whois.", StringComparison.OrdinalIgnoreCase) >= 0;
            return nodeHasWhoIs ? node.NextSibling.InnerText.Trim() : string.Empty;
        }
    }
}

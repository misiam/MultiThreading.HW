using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MultiThreading.HW.IANA.ZoneServer.Requests
{
    public class WebReader : IReader
    {
        public HtmlDocument Load(string url)
        {
            var web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            return doc;
        }
    }
}

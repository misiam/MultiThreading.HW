using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading.HW.IANA.ZoneServer.Requests
{
    public interface IReader
    {
        HtmlDocument Load(string url);
    }
}

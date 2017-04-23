using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading.HW.IANA.ZoneServer.Requests
{
    public abstract class HttpClient
    {
        private IReader _reader;
        public HttpClient()
        {
            _reader = new WebReader();
        }

        protected HtmlNodeCollection GetLinesInner(string url, string xpath)
        {
            var doc = this._reader.Load(url);
            var nodes = doc.DocumentNode.SelectNodes(xpath);
            return nodes;
        }
    }
}

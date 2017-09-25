using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using AngleSharp.Parser.Html;
using FreeProxyListLoader.Models.Enums;

namespace FreeProxyListLoader.Models.Loader
{
    class LoaderFreeProxy : ILoader<FreeProxy>
    {
        private readonly int limitConntctions;
        private int connections;

        public string Url { get; } = "https://free-proxy-list.net/";

        public IEnumerable<FreeProxy> LoadedItems { get; private set; }

        public LoaderFreeProxy(int connections = 5)
        {
            limitConntctions = connections;
        }

        private string GetSource(string url)
        {
            try
            {
                var req = WebRequest.CreateHttp(url);
                req.Timeout = 5000;

                using (var resp = req.GetResponse())
                using (var sr = new StreamReader(resp.GetResponseStream()))
                    return sr.ReadToEnd();
            }
            catch (Exception)
            {
                throw new TimeoutException();
            }
        }


        public void Load()
        {
            try
            {
                connections++;
                var src = GetSource(Url);

                // loading successfully
                LoadedItems = null;
                connections = 0;

                var htmlParser = new HtmlParser();
                var document = htmlParser.Parse(src);

                // get a proxy list
                LoadedItems = document
                    .QuerySelectorAll("table tbody tr")
                    .Select(proxy => new FreeProxy
                    {
                        Ip = proxy.QuerySelector("td:nth-child(1)").InnerHtml,
                        Port = int.Parse(proxy.QuerySelector("td:nth-child(2)").InnerHtml),
                        Code = proxy.QuerySelector("td:nth-child(3)").InnerHtml,
                        Country = proxy.QuerySelector("td:nth-child(4)").InnerHtml,
                        Anonymity =
                            proxy.QuerySelector("td:nth-child(5)").InnerHtml == "anonymous"
                                ? Anonymity.Anonymous
                                : proxy.QuerySelector("td:nth-child(5)").InnerHtml == "elite proxy"
                                    ? Anonymity.Elite
                                    : Anonymity.Transparent,
                        IsHttps = proxy.QuerySelector("td:nth-child(7)").InnerHtml != "no",
                        LastChecked = proxy.QuerySelector("td:nth-child(8)").InnerHtml
                    });
            }
            catch (TimeoutException ex)
            {
                if (connections < limitConntctions)
                    // try again...
                    Load();
                else
                    throw new Exception("Error to connection to web-site", ex);
            }
            catch (Exception ex)
            {
                throw new FormatException("web-site change structure of DOM", ex);
            }
        }
    }
}

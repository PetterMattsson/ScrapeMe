using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;

namespace ScrapeService
{
    class ScrapeClass
    {
        public DataSet Data { get; set; }
        public List<string> urls = new List<string>();

        public ScrapeClass(string url)
        {
            urls = Loop(url);
            XmlTextReader reader = new XmlTextReader(url);
            //Data = reader.ReadOuterXml();                     // turn xml-document to readable data from passed url
        }

        // Overload for more ScrapingPatterns
        public void Scrape(SPKvalster pattern)
        {
            foreach (string url in urls)
            {
                HousingObject ho = pattern.Scrape(url);           // catch object returned by scrapingpattern

                if (ho.SourceUrl != "" || ho.SourceUrl != null)     // only save if the sourceUrl is present
                {
                    SaveData(ho);
                }
            }
        }

        public List<string> Loop(string url)
        {
            //private IEnumerable<string> GetUrls(string url)
            //        {
            List<string> urls = new List<string>();
            XmlReader xmlReader = new XmlTextReader(string.Format("{0}sitemap.xml", url));
            XPathDocument document = new XPathDocument(xmlReader);
            XPathNavigator navigator = document.CreateNavigator();

            XmlNamespaceManager resolver = new XmlNamespaceManager(xmlReader.NameTable);
            resolver.AddNamespace("sitemap", url);

            XPathNodeIterator iterator = navigator.Select("/sitemap:urlset/sitemap:url/sitemap:loc", resolver);

            while (iterator.MoveNext())
            {
                if (iterator.Current == null)
                    continue;

                urls.Add(iterator.Current.Value);
            }

            return urls;
        }
        //}

        public void SaveData(HousingObject ho)
        {
            // save ho to db
        }
    }
}

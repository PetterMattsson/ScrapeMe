using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ScrapeService
{
    class ScrapeClass
    {
        public DataSet Data { get; set; }
        public List<string> urls = new List<string>();
        int NumberOfScrapes;
        int ObjectId = 1;

        public ScrapeClass(string url)
        {
            urls = Loop(url);
            XmlTextReader reader = new XmlTextReader(url);
            NumberOfScrapes = urls.Count();
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
                    ho.Id = ObjectId;
                    SaveData(ho);
                }
                ObjectId += 1;
            }
        }

        public List<string> Loop(string map)
        {
            // xml-taggen som ska hämtas är <doc>värde</doc>

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(map);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            XmlDocument doc = new XmlDocument();
            //doc.LoadXml(sr.ReadToEnd());
            string html = sr.ReadToEnd();
            //XElement sitemap = XElement.Load(sr.ReadToEnd());
            sr.Close();
            XmlReader reader = ;
            doc = reader.ReadOuterXml(); ;

            //// Download sitemap.
            XElement sitemap = XElement.Load(map);

            // ... XNames.
            XName url = XName.Get("url", "http://www.sitemaps.org/schemas/sitemap/0.9");
            XName loc = XName.Get("loc", "http://www.sitemaps.org/schemas/sitemap/0.9");

            foreach (var urlElement in sitemap.Elements(url))
            {
                var locElement = urlElement.Element(loc);
                Console.WriteLine(locElement.Value);
            }


            //private IEnumerable<string> GetUrls(string url)
            //        {
            //List<string> urls = new List<string>();
            //XmlReader xmlReader = new XmlTextReader(string.Format("{0}sitemap.xml", map));
            //XPathDocument document = new XPathDocument(xmlReader);
            //XPathNavigator navigator = document.CreateNavigator();

            //XmlNamespaceManager resolver = new XmlNamespaceManager(xmlReader.NameTable);
            //resolver.AddNamespace("sitemap", "http://www.google.com/schemas/sitemap/0.9");

            //XPathNodeIterator iterator = navigator.Select("/sitemap:urlset/sitemap:url/sitemap:loc", resolver);

            //while (iterator.MoveNext())
            //{
            //    if (iterator.Current == null)
            //        continue;

            //    urls.Add(iterator.Current.Value);
            //}

            return urls;
        }
        //}

        public void SaveData(HousingObject ho)
        {
            // save ho to db
        }
    }
}

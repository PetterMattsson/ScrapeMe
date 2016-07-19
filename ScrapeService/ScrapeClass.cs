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
        List<HousingObject> ObjectsToSave = new List<HousingObject>();

        public ScrapeClass()
        {
            //urls = Loop(url);
            //XmlTextReader reader = new XmlTextReader(url);
            NumberOfScrapes = urls.Count();
            //Data = reader.ReadOuterXml();                     // turn xml-document to readable data from passed url
        }

        // Overload for more ScrapingPatterns
        public void Scrape(SPKvalster pattern, string url)
        {
            //HousingObject ho = pattern.Scrape("http://kvalster.se/Halmstad/Uthyres/L%C3%A4genheter/Snostorpsvagen_66_1775554");
            //ObjectId = 5;
            //ho.HousingId = ObjectId.ToString();
            //ObjectsToSave.Add(ho);
            //urls = Loop(url);
            List<string> sitemaps = new List<string>();
            sitemaps = pattern.GetSiteMap(url);
            int thread = 0;
            // THREAD THIS LOOP
            foreach (string s in urls)
            {
                HousingObject ho = pattern.Scrape(url);           // catch object returned by scrapingpattern

                if (ho.SourceUrl == "" || ho.SourceUrl == null)     // only save if the sourceUrl is present
                {
                    ObjectsToSave.Remove(ho);
                }
                else
                {
                    ho.HousingId = ObjectId.ToString();
                    ObjectId += 1;
                }
            }
            SaveData(ObjectsToSave);
        }

        public List<string> Loop(string map)
        {
            // xml-taggen som ska hämtas är <doc>värde</doc>
            //XDocument doc = XDocument.Load(map);
            //SiteMapPath smp = new SiteMapPath();
            //smp.SiteMapProvider;


            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(map);
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //StreamReader sr = new StreamReader(response.GetResponseStream());
            //XmlDocument doc = new XmlDocument();
            ////doc.LoadXml(sr.ReadToEnd());
            //string html = sr.ReadToEnd();
            ////XElement sitemap = XElement.Load(sr.ReadToEnd());
            //sr.Close();
            //html = html.Replace(Convert.ToString((byte)0x1F), string.Empty);
            //XmlReader reader = new XmlTextReader(html);
            //doc = reader.ReadOuterXml(); ;

            //// Download sitemap.
            XElement sitemap = XElement.Load(map + "/Search/sitemap");

            //// ... XNames.
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
            //XElement x = XElement.Load(new StringReader(map));
            //XmlReader xmlReader = new XmlTextReader("http://www.gstatic.com/culturalinstitute/sitemaps/www_google_com_culturalinstitute/sitemap-index.xml");
            //xmlReader.Read();
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

        public void SaveData(List<HousingObject> hos)
        {
            SeachPush sp = new SeachPush(hos);
        }
    }
}

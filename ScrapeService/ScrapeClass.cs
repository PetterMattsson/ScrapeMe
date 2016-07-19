using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
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


            NumberOfScrapes = urls.Count();

        }

        // Overload for more ScrapingPatterns
        public void Scrape(SPKvalster pattern, string url)
        {
            pattern.GetSiteMap(url);
            urls = Loop(url);
            HousingObject ho = pattern.Scrape("http://kvalster.se/Halmstad/Uthyres/L%C3%A4genheter/Snostorpsvagen_66_1775554");
            ho.HousingId = 5.ToString();
            ObjectsToSave.Add(ho);
            //foreach (string url in urls)
            //{
            //    HousingObject ho = pattern.Scrape(url);           // catch object returned by scrapingpattern

            //    if (ho.SourceUrl != "" || ho.SourceUrl != null)     // only save if the sourceUrl is present
            //    {
            //        ho.Id = ObjectId;
            //        SaveData(ho);
            //    }
                ObjectId += 1;
            //}
            SaveData(ObjectsToSave);
        }

        public List<string> Loop(string map)
        {
            // xml-taggen som ska hämtas är <doc>värde</doc>
            // tänkt att fylla listan urls med alla URL:s ifrån en sitemap

            return urls;
        }


        public void SaveData(List<HousingObject> hos)
        {
            SeachService sp = new SeachService();
            //sp.DeleteIndex();
            //sp.BuildIndex();
            sp.ListUpload(hos);
        }
    }
}

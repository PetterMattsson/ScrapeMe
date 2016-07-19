using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using System.Net;

namespace ScrapeService
{
    class SPKvalster
    {
        public HousingObject Scrape(string url)
        {
            HousingObject ho = new HousingObject();

            ScrapingBrowser Browser = new ScrapingBrowser();
            Browser.AllowAutoRedirect = true;
            Browser.AllowMetaRedirect = true;

            // get page
            WebPage PageResult = Browser.NavigateToPage(new Uri(url));

            // get Nodes
            HtmlNode body = PageResult.Html.Descendants("article").First();
            HtmlNode titleNode = body.FirstChild.InnerHtml.ToHtmlNode();
            HtmlNode countyNode = PageResult.Html.CssSelect(".e").FirstOrDefault().FirstChild;
            HtmlNode areaNode = body.FirstChild.NextSibling;
            HtmlNode descriptionNode = body.FirstChild.NextSibling.NextSibling.NextSibling.NextSibling;
            HtmlNode sourceUrlNode = descriptionNode.NextSibling.NextSibling;
            HtmlNode TableNode = PageResult.Html.CssSelect(".av").FirstOrDefault().FirstChild;

            // get values from nodes
            string sourceUrl = sourceUrlNode.GetAttributeValue("href");
            string title = titleNode.FirstChild.GetAttributeValue("title");
            string category = titleNode.FirstChild.NextSibling.OuterHtml.GetFirstWord();
            string county = countyNode.FirstChild.InnerText.RemoveLastWord();
            string municipality = countyNode.FirstChild.NextSibling.InnerText.KeepMiddleWord();
            string area = areaNode.InnerText.GetLastWord();
            string address = areaNode.InnerText.GetAddress();
            string description = descriptionNode.InnerText;
            string sourceName = sourceUrlNode.InnerText;
            string fee = "";
            string size = "";
            DateTime updated = new DateTime();
            string rooms = "";
            foreach (var v in TableNode.ChildNodes)
            {
                if (v.Equals(TableNode.ChildNodes.ElementAt(0)))
                    fee = v.LastChild.InnerText.GetNumber();
                if (v.Equals(TableNode.ChildNodes.ElementAt(1)))
                    rooms = v.LastChild.InnerText;
                if (v.Equals(TableNode.ChildNodes.ElementAt(2)))
                    size = v.LastChild.InnerText.GetNumber();
                if (v.Equals(TableNode.ChildNodes.ElementAt(3)))
                    updated = v.LastChild.InnerText.GetDateTime();
            }

            // fill object with values
            ho.Address = address;
            ho.Area = area;
            ho.Category = category;
            ho.City = ""; // city;
            ho.County = county;
            ho.Description = description;
            ho.Fee = Convert.ToInt32(fee);
            ho.Municipality = municipality;
            ho.Rooms = Convert.ToInt16(rooms);
            ho.Size = Convert.ToDouble(size);
            ho.SourceName = sourceName;
            ho.SourceUrl = sourceUrl;
            ho.Title = title;
            ho.Updated = updated;

            // return object to ScrapeClass
            return ho;
        }

        public List<string> GetSiteMap(string url)
        {
            List<string> result = new List<string>();

            WebClient Client = new WebClient();
            string path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Data.txt");
            //Client.DownloadFile(url + "sitemap.xml", path);

            ScrapingBrowser Browser = new ScrapingBrowser();
            Browser.AllowAutoRedirect = true;
            Browser.AllowMetaRedirect = true;
            //Browser.TransferEncoding = "UTF-8";
            //string s = Browser.DownloadString(new Uri (url + "Search/sitemap"));
            string s = "";
            WebRequest request = WebRequest.Create(url + "sitemap");
            request.Timeout = 30 * 60 * 1000;
            request.UseDefaultCredentials = true;
            request.Proxy.Credentials = request.Credentials;
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader (stream, true))
                {
                    Encoding c = reader.CurrentEncoding;
                    s = reader.ReadToEnd();
                }
                response.Close();
            }


            s = s.Replace(Convert.ToString((byte)0x1F), "");

            //string s2 = File.ReadAllText(s);
            s = Regex.Replace(s, @"[\u0000-\u001F]", string.Empty);

            XmlDocument xml = new XmlDocument();
            //xml.GetElementsByTagName("loc");
            //string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            //if (xml.StartsWith(_byteOrderMarkUtf8))
            //{
            //    xml = xml.Remove(0, _byteOrderMarkUtf8.Length);
            //}
            byte[] encodedString = Encoding.UTF8.GetBytes(s);
            string joined = string.Join(",", encodedString.Select(x => x.ToString()).ToArray());
            using (MemoryStream ms = new MemoryStream(encodedString))
            {
                ms.Flush();
                ms.Position = 0;
                xml.Load(ms);
            }
            
            //    int index = s.IndexOf((char)0x1F);
            //if (index > 0)
            //    xml.LoadXml(s.Substring(index, s.Length - index));
            //else
            //    xml.LoadXml(s);
            //xml.Load(url + "Search/sitemap");
            XmlNamespaceManager manager = new XmlNamespaceManager(xml.NameTable);
            manager.AddNamespace("s", xml.DocumentElement.NamespaceURI); //Using xml's properties instead of hard-coded URI
            XmlNodeList xnList = xml.SelectNodes("/s:sitemapindex/s:sitemap", manager);
            // get page
            WebPage PageResult = Browser.NavigateToPage(new Uri(url + "Search/sitemap"));
            
            return result;
        }

    }
}

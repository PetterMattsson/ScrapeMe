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
using System.IO.Compression;
using System.Xml.XPath;

namespace ScrapeService
{
    class SPKvalster
    {
        public HousingObject Scrape(string url)
        {
            try
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
                ho.Description = description.Truncate(4000); // string-method som tar första 4000 chars ur strängen om den är längre än 4000 chars
                ho.Fee = fee.IsNumber();
                ho.Municipality = municipality;
                ho.Rooms = rooms.IsNumber();
                ho.Size = size.IsDouble();
                ho.SourceName = sourceName;
                ho.SourceUrl = sourceUrl;
                ho.Title = title;
                ho.Updated = updated;

                // return object to ScrapeClass
                return ho;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<List<string>> GetSiteMap(string url)
        {
            List<string> result = await url.GetNodeList();
            return result;
        }

    }
}

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

        public Xml GetSiteMap()
        {
            Xml sitemap = new Xml();
            return sitemap;
        }

    }
}

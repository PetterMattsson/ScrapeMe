using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrapySharp.Network;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ScrapeService
{
    class Program
    {
        static void Main(string[] args)
        {
            // set up Browserobject
            ScrapingBrowser Browser = new ScrapingBrowser();
            Browser.AllowAutoRedirect = true;
            Browser.AllowMetaRedirect = true;

            // get page
            WebPage PageResult = Browser.NavigateToPage(new Uri("http://localhost:65165/Index.html"));

            // get body
            HtmlNode body = PageResult.Html.Descendants("article").First();

            /*
             * BREAK OUT THE DATA WITH YOUR COCK OUT
             */

            // get title
            HtmlNode titleNode = body.FirstChild.InnerHtml.ToHtmlNode();
            string title = titleNode.FirstChild.GetAttributeValue("title");
            Console.WriteLine("Title: " + title);

            // get category
            string category = titleNode.FirstChild.NextSibling.OuterHtml.GetFirstWord();
            Console.WriteLine("Category: " + category);

            // get County
            HtmlNode countyNode = PageResult.Html.CssSelect(".e").FirstOrDefault().FirstChild;
            string county = countyNode.FirstChild.InnerText.RemoveLastWord();
            Console.WriteLine("County: " + county);

            // get Municipality
            string municipality = countyNode.FirstChild.NextSibling.InnerText.KeepMiddleWord();
            Console.WriteLine("Municipality: " + municipality);

            // get area
            HtmlNode areaNode = body.FirstChild.NextSibling;
            string area = areaNode.FirstChild.NextSibling.InnerText.GetLastWord();
            Console.WriteLine("Area: " + area);

            // get adress
            string address = areaNode.FirstChild.NextSibling.InnerText.GetFirstWord();
            Console.WriteLine("Adress: " + address);

            // get description
            HtmlNode descriptionNode = body.FirstChild.NextSibling.NextSibling.NextSibling.NextSibling;
            string description = descriptionNode.InnerText;
            Console.WriteLine("Description: " + description);

            // get sourceUrl
            HtmlNode sourceUrlNode = descriptionNode.NextSibling.NextSibling;
            string sourceUrl = sourceUrlNode.GetAttributeValue("href");
            Console.WriteLine("SourceUrl: " + sourceUrl);

            // get sourceName
            string sourceName = sourceUrlNode.InnerText;
            Console.WriteLine("SourceName: " + sourceName);

            // get fee, size, updated & rooms
            HtmlNode TableNode = PageResult.Html.CssSelect(".av").FirstOrDefault().FirstChild;

            string fee = "";
            string size = "";
            DateTime updated = new DateTime();
            string rooms = "";

            foreach (var v in TableNode.ChildNodes)             // skriver ut rätt värde ur tabellen, oformatterat.
            {
                if(v.Equals(TableNode.ChildNodes.ElementAt(0)))
                    fee = v.LastChild.InnerText.GetNumber();        // via metoden som plockar bort alla chars som inte är siffror
                if (v.Equals(TableNode.ChildNodes.ElementAt(1)))
                    rooms = v.LastChild.InnerText;
                if (v.Equals(TableNode.ChildNodes.ElementAt(2)))
                    size = v.LastChild.InnerText.GetNumber();
                if (v.Equals(TableNode.ChildNodes.ElementAt(3)))     
                    updated = v.LastChild.InnerText.GetDateTime();    // via extentionmethod som konverterar till datetime
            }
            Console.WriteLine("Fee: " + fee);
            Console.WriteLine("Rooms: " + rooms);
            Console.WriteLine("Size: " + size);
            Console.WriteLine("Updated: " + updated.ToShortDateString());

            // Nya upp ett objekt HousingObject och fyll med data -> skriv till JSON -> tanka upp

        Console.ReadKey();
        }
    }
}

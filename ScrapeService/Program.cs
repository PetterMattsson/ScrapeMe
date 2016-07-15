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
            // http://localhost:65165/Index.html

            // test med att söka i document istället
            var webGet = new HtmlWeb();
            var document = webGet.Load("http://localhost:65165/Index.html");

            HtmlNode findBody = document.DocumentNode.Descendants("article").First() ;
            HtmlNode titleNode = findBody.FirstChild.InnerHtml.ToHtmlNode();
            string title = titleNode.FirstChild.GetAttributeValue("title");

            ScrapingBrowser Browser = new ScrapingBrowser();
            Browser.AllowAutoRedirect = true;
            Browser.AllowMetaRedirect = true;

            WebPage PageResult = Browser.NavigateToPage(new Uri("http://localhost:65165/Index.html"));
            //List<HtmlNode> nodes = new List<HtmlNode>();

            // ger oss tabellen
            HtmlNode TableNode = PageResult.Html.CssSelect(".av").FirstOrDefault().FirstChild;
            //var node2 = node.ChildNodes.ElementAt(2);

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

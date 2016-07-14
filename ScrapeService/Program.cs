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

            ScrapingBrowser Browser = new ScrapingBrowser();
            Browser.AllowAutoRedirect = true;
            Browser.AllowMetaRedirect = true;

            WebPage PageResult = Browser.NavigateToPage(new Uri("http://localhost:65165/Index.html"));
            //List<HtmlNode> nodes = new List<HtmlNode>();

            // ger oss tabellen
            HtmlNode node = PageResult.Html.CssSelect(".av").FirstOrDefault().FirstChild;
            //var node2 = node.ChildNodes.ElementAt(2);

            string fee = "";
            string size = "";
            string updated = "";
            string rooms = "";

            foreach (var v in node.ChildNodes)             // skriver ut rätt värde ur tabellen, oformatterat.
            {
                if(v.Equals(node.ChildNodes.ElementAt(0)))
                    fee = GetFee(v.LastChild.InnerText);        // via metoden som plockar bort alla chars som inte är siffror
                if (v.Equals(node.ChildNodes.ElementAt(1)))
                    rooms = v.LastChild.InnerText;
                if (v.Equals(node.ChildNodes.ElementAt(2)))
                    size = v.LastChild.InnerText;
                if (v.Equals(node.ChildNodes.ElementAt(3)))     
                    updated = v.LastChild.InnerText;            // Här behöver vi göra om strängen till en datetime
            }
            Console.WriteLine("Fee: " + fee);
            Console.WriteLine("Rooms: " + rooms);
            Console.WriteLine("Size: " + size);
            Console.WriteLine("Updated: " + updated);

            // Nya upp ett objekt HousingObject och fyll med data -> skriv till JSON -> tanka upp

        Console.ReadKey();
        }

        static public string GetFee(string str)
        {
            string result = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsDigit(str, i))
                    result += str.ElementAt(i);
            }
            return result;
        }
    }
}

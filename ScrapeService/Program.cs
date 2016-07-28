using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrapySharp.Network;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Web.UI.WebControls;

namespace ScrapeService
{
    class Program
    {
        static void Main(string[] args)
        {
            Variables variables = Variables.GetVariables();
            ScrapeClass ScrapeService = new ScrapeClass();
            ScrapeService.Scrape(new SPKvalster(), "http://kvalster.se/");
            // skriv ut NumberOfScrapes och aktuellt ObjectId, fejkar en logfil
            Console.WriteLine("Number of Scrapes: " + variables.NumberOfScrapes);
            Console.WriteLine("Number of Saves to Database: " + variables.NumberOfSaves);
        Console.ReadKey();
        }
    }
}

﻿using System;
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
            ScrapeClass ScrapeService = new ScrapeClass();
            ScrapeService.Scrape(new SPKvalster(), "http://kvalster.se/");
            // skriv ut NumberOfScrapes och aktuellt ObjectId
        Console.ReadKey();
        }
    }
}

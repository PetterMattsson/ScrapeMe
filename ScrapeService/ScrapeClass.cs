using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        //int NumberOfScrapes;
        //int ObjectId = 1;
        List<HousingObject> ObjectsToSave = new List<HousingObject>();
        string table = "";
        string conString = "Server = tcp:scraperesultserver.database.windows.net,1433; Initial Catalog = ScrapeResults; Persist Security Info = False; User ID = scraperesultlogin; Password =B1g02016; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;";

        public ScrapeClass()
        {
            table = GetTable();

            

        }

        public string GetTable()
        {
            string str = "";
            if (table == "")
            {
                table = "HousingObjects";
            }
            else
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    using (SqlCommand com = new SqlCommand("Select TableName from TableToUse", con))
                    {
                        // hämta vilken tabell vi ska använda
                        str = com.ExecuteScalar().ToString();
                    }
                    con.Close();
                }
            }
            return str;
        }

        public void SetTable(string str)
        {
            string tn = table == "HousingObjects" ? "HousingObjectsAlternate" : "HousingObjects";
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand com = new SqlCommand("Update TableToUse Set TableName = " + tn))
                {
                    com.ExecuteNonQuery();
                }
                con.Close();
            }
        }


        // Overload for more ScrapingPatterns
        public async void Scrape(SPKvalster pattern, string url)
        {
            List<string> maps = pattern.GetSiteMap(url + @"\sitemap");
            //XMLMethods.WriteToData(url + @"\sitemap");
            //List<string> maps = XMLMethods.ReadFromData();
            urls = await Loop(maps);

            foreach (string link in urls)
            {
                HousingObject ho = pattern.Scrape(link);           // catch object returned by scrapingpattern

                if (ho.SourceUrl != "" || ho.SourceUrl != null)     // only save if the sourceUrl is present
                {
                    ho.HousingId = Variables.GetObjectId().ToString();
                    Variables.IncrementObjectId();
                }
            }
            SaveData(ObjectsToSave);
        }

        public async Task< List<string>> Loop(List<string> sitemaps)
        {
            /*
             *      DET ÄR DEN FÖRSTA LOOPEN HÄR SOM VI BEHÖVER TRÅDA
             */

            DateTime start = DateTime.Now;
            // xml-taggen som ska hämtas är <loc>värde</loc>
            //fyller listan urls med alla URL:s ifrån ett sitemapindex som vi får ifrån ScrapingPattern
            List<List<string>> sites = new List<List<string>>();
            int i = 0;
            foreach (string map in sitemaps)
            {
                bool success = await XMLMethods.WriteToData(map);
                //System.Threading.Thread.Sleep(3000);
                if (success)
                {
                    List<string> tmp = XMLMethods.ReadFromData();
                    List<string> tmp2 = new List<string>();
                    i = tmp.Count();
                    foreach (string site in tmp)
                    {
                        urls.Add(site);
                    }
                    Console.WriteLine(map + ": " + i);
                    Console.WriteLine("Totalt: " + urls.Count());
                }
                //sites.Add(tmp2);
            //}
            //foreach (List<string> list in sites)
            //{
            //    foreach (string str in list)
            //    {
            //        urls.Add(str);
            //    }
            }
            DateTime end = DateTime.Now;
            TimeSpan tid = end - start;
            Console.WriteLine("urls byggdes på " + tid + ", med " + urls.Count() + " stycken länkar.");
            Variables.AddScrapes(urls.Count());
            return urls;
        }


        public void SaveData(List<HousingObject> hos)
        {
            bool newTable = false;
            List<HousingObjectID> hosID = new List<HousingObjectID>();
            table = newTable ? "HousingObjectsAlternate" : "HousingObjects";

            // Konvertera till objekttyp som matchar databasen
            foreach (var ho in hos)
            {
                HousingObjectID hoID = ho.ConvertToInt();
                hosID.Add(hoID);
            }

            string conString = "Server = tcp:scraperesultserver.database.windows.net,1433; Initial Catalog = ScrapeResults; Persist Security Info = False; User ID = scraperesultlogin; Password =B1g02016; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;";
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand com = new SqlCommand("Delete from " + table, con))
                {
                    // rensa data ur tabellen vi ska använda
                    com.ExecuteNonQuery();
                }
                if (!newTable)
                {
                    //string table = "HousingObjects";
                    using (SqlCommand com = new SqlCommand("", con))
                    {
                        com.CommandText = "insert into " + table + "(Title, Description, Category, Rooms, Fee, Size, Area, City, Municipality, County, Updated, Address, SourceUrl, SourceName) values (@Title, @Description, @Category, @Rooms, @Fee, @Size, @Area, @City, @Municipality, @County, @Updated, @Address, @SourceUrl, @SourceName)";

                        foreach (var ho in hosID)
                        {
                            // hur gör jag för att knö in ett helt objekt i en insert? att stega igenom properties är drygt

                            // Fyll i objektspecifika parametrar till SqlCommand
                            com.Parameters.AddWithValue("@Title", ho.Title);
                            com.Parameters.AddWithValue("@Description", ho.Description);
                            com.Parameters.AddWithValue("@Category", ho.Category);
                            com.Parameters.AddWithValue("@Rooms", ho.Rooms);
                            com.Parameters.AddWithValue("@Fee", ho.Fee);
                            com.Parameters.AddWithValue("@Size", ho.Size);
                            com.Parameters.AddWithValue("@Area", ho.Area);
                            com.Parameters.AddWithValue("@City", ho.City);
                            com.Parameters.AddWithValue("@Municipality", ho.Municipality);
                            com.Parameters.AddWithValue("@County", ho.County);
                            com.Parameters.AddWithValue("@Updated", ho.Updated);
                            com.Parameters.AddWithValue("@Address", ho.Address);
                            com.Parameters.AddWithValue("@SourceUrl", ho.SourceUrl);
                            com.Parameters.AddWithValue("@SourceName", ho.SourceName);

                            // Gör anropet till databasen
                            com.ExecuteNonQuery();
                            Variables.IncrementSaves();
                        }
                    }
                }
                if (newTable)
                {
                    //string table = "HousingObjectsAlternate";
                    using (SqlCommand com = new SqlCommand("", con))
                    {
                        com.CommandText = "insert into " + table + "(Title, Description, Category, Rooms, Fee, Size, Area, City, Municipality, County, Updated, Address, SourceUrl, SourceName) values (@Title, @Description, @Category, @Rooms, @Fee, @Size, @Area, @City, @Municipality, @County, @Updated, @Address, @SourceUrl, @SourceName)";
                        foreach (var ho in hosID)
                        {
                            // hur gör jag för att knö in ett helt objekt i en insert? att stega igenom properties är drygt

                            // Fyll i objektspecifika parametrar till SqlCommand
                            com.Parameters.AddWithValue("@Title", ho.Title);
                            com.Parameters.AddWithValue("@Description", ho.Description);
                            com.Parameters.AddWithValue("@Category", ho.Category);
                            com.Parameters.AddWithValue("@Rooms", ho.Rooms);
                            com.Parameters.AddWithValue("@Fee", ho.Fee);
                            com.Parameters.AddWithValue("@Size", ho.Size);
                            com.Parameters.AddWithValue("@Area", ho.Area);
                            com.Parameters.AddWithValue("@City", ho.City);
                            com.Parameters.AddWithValue("@Municipality", ho.Municipality);
                            com.Parameters.AddWithValue("@County", ho.County);
                            com.Parameters.AddWithValue("@Updated", ho.Updated);
                            com.Parameters.AddWithValue("@Address", ho.Address);
                            com.Parameters.AddWithValue("@SourceUrl", ho.SourceUrl);
                            com.Parameters.AddWithValue("@SourceName", ho.SourceName);

                            // Gör anropet till databasen
                            com.ExecuteNonQuery();
                            Variables.IncrementSaves();
                        }
                    }
                }
                newTable = !newTable;
                con.Close();
            }

            // Överföring till databas är klar. Push till search med nyhämtad data är som vanligt nedan
            SeachService sp = new SeachService();
            //sp.DeleteIndex();
            //sp.BuildIndex();
            sp.ListUpload(hos);
        }
    }
}

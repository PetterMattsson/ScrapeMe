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
            bool insert = true;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand com = new SqlCommand("Select TableName from TableToUse where TableId = 1", con))
                {
                    // hämta vilken tabell vi ska använda
                    object obj = com.ExecuteScalar();
                    if (obj != null && DBNull.Value != obj)
                    {
                        str = obj.ToString();
                        insert = false;
                    }
                    else
                        str = "HousingObjects";
                    com.Dispose();
                }
                if (insert)
                {
                    using (SqlCommand com = new SqlCommand("Insert into TableToUse values ('" + str + "')", con))
                    {
                        com.ExecuteNonQuery();
                        com.Dispose();
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
                using (SqlCommand com = new SqlCommand("Update TableToUse Set TableName = '" + tn + "' where TableId = 1", con))
                {
                    com.ExecuteNonQuery();
                }
                con.Close();
            }
        }


        // Overload for more ScrapingPatterns
        public async void Scrape(SPKvalster pattern, string url)
        {
            try
            {
                List<string> maps = await pattern.GetSiteMap(url + @"/sitemap");
                urls = await Loop(maps);
                urls = await urls.CleanUrls();
                DateTime start = DateTime.Now;
                for (int i = 0; i < urls.Count(); i++)
                {
                    string link = urls.ElementAt(i);
                    HousingObject ho = pattern.Scrape(link);            // catch object returned by scrapingpattern
                    if (ho == null)
                        continue;
                    if (ho.SourceUrl.Length > 0)                        // only save if the sourceUrl is present TODO: Ping webpage method!!!!
                    {
                        //if(await XMLMethods.PingWebPage(ho.SourceUrl))
                        //{
                        ObjectsToSave.Add(ho);
                        int j = ObjectsToSave.Count();
                        Console.WriteLine("Objekt (" + j + "): " + link + " lades till. Totala scrapes: " + i);
                        //}
                    }
                    //if (ObjectsToSave.Count > 5)
                    //    break;
                }
                DateTime end = DateTime.Now;
                TimeSpan time = start - end;
                Console.WriteLine("Scrape ran successfully, finishing in " + time.Duration() + ".");
                Console.WriteLine("Pushing " + ObjectsToSave.Count + " objects to database and search.");

                SaveData(ObjectsToSave);
            }
            catch (Exception e)
            {
                Console.WriteLine("ScrapeClass.Scrape() error: " + e.Message);
            }
        }

        public void ScrapeThis(SPKvalster pattern, string url)
        {
            HousingObject ho = pattern.Scrape(url);

            if (ho.SourceUrl != "" || ho.SourceUrl != null)
            {
                ObjectsToSave.Add(ho);
            }
            SaveData(ObjectsToSave);
        }



        public async Task<List<string>> Loop(List<string> sitemaps)
        {
            DateTime start = DateTime.Now;

            //fyller listan urls med alla URL:s ifrån ett sitemapindex som vi får ifrån ScrapingPattern
            List<List<string>> sites = new List<List<string>>();
            int i = 0;
            int j = 1;
            foreach (string map in sitemaps)
            {
                bool success = await XMLMethods.WriteToData(map);
                if (success)
                {
                    List<string> tmp = await XMLMethods.ReadFromData();

                    i = tmp.Count();
                    foreach (string site in tmp)
                    {
                        //if(await XMLMethods.PingWebPage(site))
                        urls.Add(site);
                    }
                    Console.WriteLine(map + ": " + i);
                    Console.WriteLine("Totalt: " + urls.Count());
                }
                else
                {
                    Console.WriteLine("Getting urls from: " + map + " failed (" + j + ").");
                    j++;
                }
            }
            DateTime end = DateTime.Now;
            TimeSpan tid = end - start;
            Console.WriteLine("urls byggdes på " + tid.TotalSeconds + ", med " + urls.Count() + " stycken länkar.");
            Console.WriteLine("Antal fel: " + j);
            Variables.AddScrapes(urls.Count());
            return urls;
        }

        public async void SaveData(List<HousingObject> hos)
        {
            List<HousingObjectID> hosID = new List<HousingObjectID>();

            // Konvertera till objekttyp som matchar databasen
            foreach (var ho in hos)
            {
                HousingObjectID hoID = ho.ConvertToInt();
                hosID.Add(hoID);
            }

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand com = new SqlCommand("TRUNCATE TABLE [" + table + "];", con))
                {
                    // rensa data ur tabellen vi ska använda
                    com.CommandTimeout = 90000;
                    await com.ExecuteNonQueryAsync();
                    com.Dispose();
                }
                foreach (var ho in hosID)
                {
                    using (SqlCommand com = new SqlCommand("", con))
                    {
                        com.CommandText = "insert into " + table + "(Title, Description, Category, Rooms, Fee, Size, Area, City, Municipality, County, Updated, Address, SourceUrl, SourceName) values (@Title, @Description, @Category, @Rooms, @Fee, @Size, @Area, @City, @Municipality, @County, @Updated, @Address, @SourceUrl, @SourceName)";


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
                        com.Dispose();

                    }

                }
                con.Close();
            }
            SetTable(table);

            // Överföring till databas är klar. Push till search med nyhämtad data är som vanligt nedan
            List<HousingObject> tmpHos = new List<HousingObject>();
            // Hämta IDn från databasen, så att de åker med till SearchPush
            SeachService sp = new SeachService();
            //Rensar gammalt
            sp.DeleteIndex();
            //Bygger nytt och ska sätta CORS
            sp.BuildIndex();

            //Fyller på det nybyggda tomma indexet
            for (int i = 0; i < hos.Count(); i++)
            {
                HousingObject ho = hos.ElementAt(i);
                ho.HousingId = ho.GetID(conString, table);
                tmpHos.Add(ho);
                if(tmpHos.Count % 1000 == 0 || i == hos.Count() -1)
                {
                    sp.ListUpload(tmpHos);
                    tmpHos.Clear();
                }
            }
            
        }
    }
}

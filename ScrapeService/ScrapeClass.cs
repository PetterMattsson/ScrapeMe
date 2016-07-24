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
        int NumberOfScrapes;
        int ObjectId = 1;
        List<HousingObject> ObjectsToSave = new List<HousingObject>();

        public ScrapeClass()
        {


            NumberOfScrapes = urls.Count();

        }

        // Overload for more ScrapingPatterns
        public void Scrape(SPKvalster pattern, string url)
        {
            //pattern.GetSiteMap(url);
            //urls = Loop(url);
            HousingObject ho = pattern.Scrape("http://kvalster.se/Halmstad/Uthyres/L%C3%A4genheter/Snostorpsvagen_66_1775554");
            ho.HousingId = ObjectId.ToString();
            ObjectsToSave.Add(ho);
            //foreach (string url in urls)
            //{
            //    HousingObject ho = pattern.Scrape(url);           // catch object returned by scrapingpattern

            //    if (ho.SourceUrl != "" || ho.SourceUrl != null)     // only save if the sourceUrl is present
            //    {
            //        ho.Id = ObjectId;
            //        SaveData(ho);
            //    }
            ObjectId += 1;
            //}
            SaveData(ObjectsToSave);
        }

        public List<string> Loop(string sitemap)
        {
            // xml-taggen som ska hämtas är <doc>värde</doc>
            // tänkt att fylla listan urls med alla URL:s ifrån en sitemap

            return urls;
        }


        public void SaveData(List<HousingObject> hos)
        {
            bool newTable = false;
            List<HousingObjectID> hosID = new List<HousingObjectID>();
            string table = newTable ? "HousingObjectsAlternate" : "HousingObjects";

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
                    using (SqlCommand com = new SqlCommand("",con))
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
                        }
                    }
                }
                if(newTable)
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

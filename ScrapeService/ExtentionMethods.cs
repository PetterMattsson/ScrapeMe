using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ScrapeService
{
    public static class ExtentionMethods
    {
        // STRING METHODS
        static public string GetNumber(this string str)
        {
            string result = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsDigit(str, i))
                    result += str.ElementAt(i);
            }
            return result;
        }

        static public DateTime GetDateTime(this string str)
        {
            try
            {
                return DateTime.Parse(str);
            }
            catch (Exception e)
            {
                // keep calm and carry on
            }
            if (str.Contains("Idag"))
            {
                return DateTime.Now.Date;
            }
            else if (str.Contains("Igår"))
            {
                return DateTime.Now.AddDays(-1);
            }
            else
            {
                string[] splitString = Regex.Split(str, @"(\s)");
                int days = Convert.ToInt32(splitString.ElementAt(0));
                return DateTime.Now.AddDays(-days);
            }
        }

        public static string GetFirstWord(this string str)
        {
            string[] splitString = Regex.Split(str, @"(\s|,)");
            return splitString.ElementAt(0);
        }

        public static string GetLastWord(this string str)
        {
            string[] splitString = Regex.Split(str, @"(\s|,)");
            int i = splitString.Count();
            return splitString.ElementAt(i - 1);
        }

        public static string RemoveLastWord(this string str)
        {
            return str.Substring(0, str.LastIndexOf(" ")).Trim();
        }

        public static string KeepMiddleWord(this string str)
        {
            string[] splitString = Regex.Split(str, @"(\s)");
            return splitString.ElementAt(2);
        }

        public static string GetAddress(this string str)
        {
            int i = str.LastIndexOf(",");
            if (i > 1)
                return str.Substring(0, i);
            else
                return str;
        }

        public static async Task<List<string>> GetNodeList(this string str)
        {
            List<string> result = new List<string>();

            bool success = await XMLMethods.WriteToData(str);

            if (success)
            {
                result = await XMLMethods.ReadFromData();
            }

            return result;
        }

        public static int IsNumber(this string str)
        {
            int i = 0;
            try
            {
                i = Convert.ToInt32(str);

            }
            catch
            {
                // do nothing
            }

            return i;

        }

        public static double IsDouble(this string str)
        {
            double d = 0;

            try
            {
                d = Convert.ToDouble(str);
            }
            catch
            {

            }
            return d;
        }




        // LIST<STRING> METHODS
        public static List<string> CleanUrls(this List<string> list)
        {
            List<string> result = new List<string>();
            foreach (string s in list)
            {
                int count = Regex.Matches(s, @"/").Count;
                if (count < 4 || s.Contains("sitemap"))
                {
                    // do nothing
                }
                else
                    result.Add(s);
            }
            return result;
        }


        // HOUSINGOBJECTS METHODS
        public static HousingObjectID ConvertToInt(this HousingObject ho)
        {
            HousingObjectID hoID = new HousingObjectID
            {
                HousingId = Convert.ToInt32(ho.HousingId),
                Address = ho.Address,
                Area = ho.Area,
                Category = ho.Category,
                City = ho.City,
                County = ho.County,
                Description = ho.Description,
                Fee = ho.Fee,
                Municipality = ho.Municipality,
                Rooms = ho.Rooms,
                Size = ho.Size,
                SourceName = ho.SourceName,
                SourceUrl = ho.SourceUrl,
                Title = ho.Title,
                Updated = ho.Updated
            };
            return hoID;
        }

        public static string GetID(this HousingObject ho, string conString, string table)
        {
            string result = "";

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand com = new SqlCommand("Select HousingID from " + table +
                    " where SourceUrl = '" + ho.SourceUrl + "' and Rooms = " + ho.Rooms + " and Fee = " + ho.Fee + "", con))
                {
                    // hämta ID från db
                    result = com.ExecuteScalar().ToString();
                }
                con.Close();
            }

            return result;
        }
    }
}

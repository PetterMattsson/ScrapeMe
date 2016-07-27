using System;
using System.Collections.Generic;
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

        public static List<string> GetNodeList(this string str)
        {
            List<string> result = new List<string>();

            WebClient client = new WebClient();
            string root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string file = string.Concat(root + @"\Data\data.txt");

            // Decompress sitemap and write to file     ---> TODO: keep in memorystream instead of file
            using (Stream stream = client.OpenRead(str))
            using (Stream tmpFile = File.Create(file))
            using (Stream compStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                byte[] buffer = new byte[4096];
                int nRead;
                while ((nRead = compStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    tmpFile.Write(buffer, 0, nRead);
                }
            }
            string[] ps = Directory.GetFiles(root + @"\Data");
            string filepath = ps.ElementAt(0);

            XmlDocument doc = new XmlDocument();
            doc.Load(filepath);
            XmlElement docRoot = doc.DocumentElement;

            XmlNodeList nodes = docRoot.GetElementsByTagName("loc");
            foreach (XmlNode node in nodes)
            {
                result.Add(node.InnerText);
            }

            return result;
        }




        // HousingObject METHODS
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
    }
}

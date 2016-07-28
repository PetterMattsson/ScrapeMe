using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ScrapeService
{
    static class XMLMethods
    {
        public static async Task<bool> WriteToData(string str)
        {
            WebClient client = new WebClient();
            string root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string file = string.Concat(root + @"\Data\data.txt");

            // Decompress sitemap and write to file     ---> TODO: keep in memorystream instead of file
            try
            {
                using (Stream stream = client.OpenRead(str))
                using (Stream tmpFile = File.Create(file))
                using (Stream compStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[1024];
                    int nRead;
                    if (buffer.Length < 1 || buffer == null)
                    {
                        return false;
                    }
                    while ((nRead = compStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        tmpFile.Write(buffer, 0, nRead);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public static List<string> ReadFromData()
        {
            List<string> result = new List<string>();

            string root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

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



}
}

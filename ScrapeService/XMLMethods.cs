using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(str);
            httpReq.AllowAutoRedirect = false;

            HttpWebResponse httpRes = (HttpWebResponse)httpReq.GetResponse();

            if (httpRes.StatusCode == HttpStatusCode.OK)
            {
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
                            httpRes.Close();
                            return false;
                        }
                        while ((nRead = compStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            tmpFile.Write(buffer, 0, nRead);
                        }
                        stream.Close();
                        stream.Dispose();
                        tmpFile.Close();
                        tmpFile.Dispose();
                        compStream.Close();
                        compStream.Dispose();
                    }
                    httpRes.Close();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    httpRes.Close();
                    return false;
                }
            }
            else
            {
                httpRes.Close();
                return false;
            }

        }

        public static async Task<List<string>> ReadFromData()
        {
            List<string> result = new List<string>();

            string root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            string[] ps = Directory.GetFiles(root + @"\Data");
            string filepath = ps.ElementAt(0);

            XmlDocument doc = new XmlDocument();
            try
            {
                //doc.Load(filepath);
                using (Stream s = File.OpenRead(filepath))
                {
                    doc.Load(s);
                    s.Close();
                    s.Dispose();
                }

                XmlElement docRoot = doc.DocumentElement;

                XmlNodeList nodes = docRoot.GetElementsByTagName("loc");
                foreach (XmlNode node in nodes)
                {
                    DateTime dt = node.NextSibling.InnerText.GetDateTime();
                    if (dt > DateTime.Now.AddMonths(-12))
                        result.Add(node.InnerText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            result.RemoveRange(0, 1);
            return result;
        }

        public static async Task<bool> PingWebPage(string url)
        {
            bool result = await Task.Run(() =>
            {
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(url);
                httpReq.AllowAutoRedirect = false;

                HttpWebResponse httpRes = (HttpWebResponse)httpReq.GetResponse();

                if (httpRes.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                    return false;
            });
            return result;
        }
    }
}

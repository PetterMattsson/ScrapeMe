using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            else
            {
                string[] splitString = Regex.Split(str, @"(\s)");
                int days = Convert.ToInt32(splitString.ElementAt(0));
                return DateTime.Now.AddDays(-days);
            }
        }
    }
}

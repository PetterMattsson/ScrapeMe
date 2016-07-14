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
            //DateTime result = new DateTime();
            try
            {
                DateTime result = DateTime.Parse(str);
                return result;
            }
            catch (Exception e)
            {
                // keep calm and carry on
            }
            int year = 0;
            int month = 0;
            int day = 0;
            if (str.Contains("Idag"))
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
                day = DateTime.Now.Day;
            }
            else
            {
                // hämta år, månad och dag ur strängen som nedan ...
            }
            string[] splitString = Regex.Split(str, @"(:|\s)");
            int hour = Convert.ToInt32(splitString.ElementAt(0));
            int minute = Convert.ToInt32(splitString.ElementAt(2));
            DateTime result2 = new DateTime(year, month, day, hour, minute, 0);
            return result2;
        }
    }
}

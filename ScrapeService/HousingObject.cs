using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeService
{
    class HousingObject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public short Rooms { get; set; }
        public int Fee { get; set; }
        public double Size { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string Municipality { get; set; }
        public string County { get; set; }
        public DateTime Updated { get; set; }
        public string Address { get; set; }
        public string SourceUrl { get; set; }
        public string SourceName { get; set; }



    }
}

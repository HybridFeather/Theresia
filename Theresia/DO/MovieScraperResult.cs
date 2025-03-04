using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresia.DO
{
    public class MovieScraperResult
    {
        public string Title { get; set; }

        public string Director { get; set; }

        public string ReleaseDate { get; set; }

        public string Manufacturer { get; set; }

        public string Distributor { get; set; }

        public List<string> TagList { get; set; }

        public List<string> ActorList { get; set; }
        public string Series { get; internal set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p31approve
{
    public class GridRecord
    {
        public string Datum { get; set; }
        public string Jmeno { get; set; }
        public string HodinyKFakturaci { get; set; }
        public string HodinyVPausaulu { get; set; }
        public double bezdph { get; set; }
        public double sazba { get; set; }
        public string Popis { get; set; }
        public string Projekt { get; set; }
        public string pl { get; set; }
        public string Aktivita { get; set; }
        public int p33ID { get; set; }
        public int p72ID { get; set; }
    }
}

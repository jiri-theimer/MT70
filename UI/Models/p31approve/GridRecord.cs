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
        public string hodiny { get; set; }
        public string hodinypausal { get; set; }
        public string hodinyinterni { get; set; }
        public double bezdph { get; set; }
        public double dphsazba { get; set; }
        public double sazba { get; set; }
        public string Popis { get; set; }
        public string Projekt { get; set; }
        public string pl { get; set; }
        public string Aktivita { get; set; }
        public string vykazano { get; set; }
        public string vykazano_sazba { get; set; }
        public string j27code { get; set; }
        public int p33id { get; set; }
        public int p71id { get; set; }
        public int p72id { get; set; }
        public int uroven { get; set; }

        public string errormessage { get; set; }
    }
}

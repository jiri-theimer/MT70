using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XA.Models.sepa
{
    public class SepaViewModel
    {
        public string Guid { get; set; }
        public DateTime DatumSplatnosti { get; set; }
        public string Vystavovatel { get; set; }
        public string VystavovatelUlice { get; set; }
        public string VystavovatelMesto { get; set; }
        public string PrijemceIBAN { get; set; }
        public string PrijemceBIC { get; set; }
        public string PrijemceCID { get; set; }

        public string GeneratedFileName { get; set; }
        
    }
}

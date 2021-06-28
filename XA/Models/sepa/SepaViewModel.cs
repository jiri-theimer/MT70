using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace XA.Models.sepa
{
    public class XlsRecord
    {
        public string ClientID { get; set; }
        public DateTime PodepsaniMandatu { get; set; }
        public string BIC { get; set; }
        public string Jmeno { get; set; }
        public string IBAN { get; set; }
        
        public string FakturaVS { get; set; }
        public double FakturaCastka { get; set; }
        public string FakturaMena { get; set; }
    }
    public class SepaViewModel
    {
        public List<XlsRecord> lisRecords { get; set; }
        public string Guid { get; set; }
        public string GuidImport{get;set; }
        public IFormFile FileforImport { get; set; }
        public string ImportOrigFileName { get; set; }
        
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public enum p92InvoiceTypeENUM
    {
        ClientInvoice = 1,
        CreditNote = 2
    }
    public class p92InvoiceT:BaseBO
    {
        public p92InvoiceTypeENUM p92InvoiceType { get; set; } = p92InvoiceTypeENUM.ClientInvoice;
        public string p92Name { get; set; }
        public string p92Code { get; set; }
        public int p93ID { get; set; }
        public int x31ID_Invoice { get; set; }
        public int x31ID_Attachment { get; set; }
        public int x31ID_Letter { get; set; }
        public int j27ID { get; set; }
        public int j17ID { get; set; }
        public int p98ID { get; set; }
        public x15IdEnum? x15ID { get; set; }
        public int j19ID { get; set; }
        public int b01ID { get; set; }
        public int x38ID { get; set; }
        public int x38ID_Draft { get; set; }
        public int p80ID { get; set; }
        public int j61ID { get; set; }
        public int p92Ordinary { get; set; }

        public string p92ReportConstantPreText1 { get; set; }
        public string p92InvoiceDefaultText1 { get; set; }
        public string p92InvoiceDefaultText2 { get; set; }
        public string p92ReportConstantText { get; set; }

        public string p92AccountingIDS { get; set; }  // kód předkontace v účetním IS
        public string p92ClassificationVATIDS { get; set; }   // kód klasifikace DPH v účetním IS

        public string j27Code { get; }        
        public string j17Name { get; }       
        public string j61Name { get; }        
        public string p93Name { get; }     
        public string x15Name { get; }
        
      
    }
}

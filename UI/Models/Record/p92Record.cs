using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p92Record:BaseRecordViewModel
    {
        public BO.p92InvoiceT Rec { get; set; }
        public string ComboJ17Name { get; set; }
        public string ComboJ27Code { get; set; }
        public string ComboX31_Report { get; set; }
        public string ComboX31_Attachment { get; set; }
        public string ComboX31_Letter { get; set; }
        public string ComboX38Name { get; set; }
        public string ComboP93Name { get; set; }
        public string ComboP98Name { get; set; }
        public string ComboP80Name { get; set; }
        public string ComboJ61Name { get; set; }
        
        
    }
}

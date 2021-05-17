using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab1
{
    public class p91Tab1:BaseTab1ViewModel
    {
        public BO.p91Invoice Rec { get; set; }
        public BO.p93InvoiceHeader RecP93 { get; set; }
        public BO.p92InvoiceT RecP92 { get; set; }
        public BO.p86BankAcc RecP86 { get; set; }
        public IEnumerable<BO.p91_CenovyRozpis> lisCenovyRozpis { get; set; }

        public IEnumerable<BO.p31Worksheet> lisP31 { get; set; }

        public string StatByPrefix { get; set; }

        public BO.p91Invoice RecOpravovanyDoklad { get; set; }

        public IEnumerable<BO.p99Invoice_Proforma> lisP99 { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p95Record:BaseRecordViewModel
    {
        public BO.p95InvoiceRow Rec { get; set; }
        public IEnumerable<BO.p87BillingLanguage> lisP87 { get; set; }
    }
}

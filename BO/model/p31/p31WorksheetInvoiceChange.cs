using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class p31WorksheetInvoiceChange
    {
        public int p31ID { get; set; }
        public BO.p33IdENUM p33ID { get; set; }
        public BO.p70IdENUM p70ID { get; set; }
        public double InvoiceValue { get; set; }
        public double InvoiceRate { get; set; }
        public double InvoiceVatRate { get; set; }
        public string TextUpdate { get; set; }
        public double FixPriceValue { get; set; }
        public bool p31IsInvoiceManual { get; set; }
        public double ManualFee { get; set; }
        public int p32ManualFeeFlag { get; set; } = 0;
        public string p31Code { get; set; }
    }
}

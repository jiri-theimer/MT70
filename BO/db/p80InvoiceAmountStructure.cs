using System;

namespace BO
{
    public class p80InvoiceAmountStructure:BaseBO
    {
        public string p80Name { get; set; }
        public bool p80IsTimeSeparate { get; set; }
        public bool p80IsFeeSeparate { get; set; }
        public bool p80IsExpenseSeparate { get; set; }
    }
}

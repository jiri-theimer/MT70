using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p34Record:BaseRecordViewModel
    {
        public BO.p34ActivityGroup Rec { get; set; }
        public IEnumerable<BO.p87BillingLanguage> lisP87 { get; set; }


        public bool IsOffBilling { get; set; }
        public int p32ID { get; set; }
        public string ComboP32Name { get; set; }
    }
}

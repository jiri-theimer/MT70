using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p51Record:BaseRecordViewModel
    {
        public BO.p51PriceList Rec { get; set; }
        public string ComboJ27Code { get; set; }
        public List<BO.p52PriceList_Item> lisP52 { get; set; }
    }
}

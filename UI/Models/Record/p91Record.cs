using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p91Record: BaseRecordViewModel
    {
        public BO.p91Invoice Rec { get; set; }
        public string ComboJ27Code { get; set; }
        public string ComboP28Name { get; set; }
        public string ComboOwner { get; set; }
        public string ComboP80Name { get; set; }
        public string ComboP63Name { get; set; }
        public string ComboP92Name { get; set; }

        public FreeFieldsViewModel ff1 { get; set; }

        public bool CanEditRecordCode { get; set; }
    }
}

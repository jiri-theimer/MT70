using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class j02Record: BaseRecordViewModel
    {
        public BO.j02Person Rec { get; set; }

        public string ComboJ07Name { get; set; }
        public string ComboC21Name { get; set; }
        public string ComboJ18Name { get; set; }

        
        public string p34Names { get; set; }

        public int RadioIsIntraPerson { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class x28Record:BaseRecordViewModel
    {
        public BO.x28EntityField Rec { get; set; }

        public string ComboX27Name { get; set; }

        public string SelectedJ04IDs { get; set; }
        public string SelectedJ04Names { get; set; }
        public string SelectedJ07IDs { get; set; }
        public string SelectedJ07Names { get; set; }
    }
}

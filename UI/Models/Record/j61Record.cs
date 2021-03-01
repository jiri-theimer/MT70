using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class j61Record:BaseRecordViewModel
    {
        public BO.j61TextTemplate Rec { get; set; }

        public string ComboOwner { get; set; }
    }
}

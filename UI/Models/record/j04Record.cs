using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class j04Record:BaseRecordViewModel
    {
        public BO.j04UserRole Rec { get; set; }
        public List<int> SelectedX53IDs { get; set; }
        public IEnumerable<BO.x53Permission> lisAllX53 { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p42Record:BaseRecordViewModel
    {
        public BO.p42ProjectType Rec { get; set; }
        public List<int> SelectedP34IDs { get; set; }
        public IEnumerable<BO.p34ActivityGroup> lisAllP34 { get; set; }

        public string ComboP07Name { get; set; }
        public string ComboB01Name { get; set; }
        public string ComboX38Name { get; set; }
    }
}

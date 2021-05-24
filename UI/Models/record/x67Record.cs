using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class x67Record:BaseRecordViewModel
    {
        public BO.x67EntityRole Rec { get; set; }

        
        public List<int> SelectedX53IDs { get; set; }
        public IEnumerable<BO.x53Permission> lisAllX53 { get; set; }

        public int x29ID { get; set; }

        public List<BO.o28ProjectRole_Workload> lisO28 { get; set; }

        public IEnumerable<BO.x67EntityRole> lisDisponibleSlaves { get; set; }
        public List<int> SelectedSlaves { get; set; }
    }
}

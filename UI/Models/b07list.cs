using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class b07list: BaseViewModel
    {
        public string master_entity { get; set; }
        public int master_pid { get; set; }
        public IEnumerable<BO.b07Comment> lisB07 { get; set; }

        public IEnumerable<BO.o27Attachment> lisO27 { get; set; }
    }
}

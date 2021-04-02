using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class o38Record:BaseRecordViewModel
    {
        public BO.o38Address Rec { get; set; }

        public int p28ID { get; set; }
        public int o36ID { get; set; }
        public string TempGuid { get; set; }
    }
}

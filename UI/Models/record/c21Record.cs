using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class c21Record:BaseRecordViewModel
    {
        public BO.c21FondCalendar Rec { get; set; }

        public List<BO.c28FondCalendar_Log> lisC28 { get; set; }
    }
}

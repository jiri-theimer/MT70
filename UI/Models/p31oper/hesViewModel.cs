using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p31oper
{
    public class hesViewModel:BaseViewModel
    {
        public int TotalFlagValue { get; set; }

        public int HoursInterval { get; set; }
        public string HoursFormat { get; set; }

        public bool TimesheetEntryByMinutes { get; set; }
    }
}

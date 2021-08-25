using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p31oper
{
    public class hesViewModel:BaseViewModel
    {
        public string PageSource { get; set; }
        public int TotalFlagValue { get; set; }

        public int HoursInterval { get; set; }
        public string HoursFormat { get; set; }

        public bool TimesheetEntryByMinutes { get; set; }
        public bool OfferTrimming { get; set; }
        public bool OfferContactPerson { get; set; }

        public void InhaleSetting() //odvodit parametry podle hodnoty TotalFlagValue
        {
            if (BO.BAS.bit_compare_or(this.TotalFlagValue, 2)) this.HoursInterval = 30;
            if (BO.BAS.bit_compare_or(this.TotalFlagValue, 4)) this.HoursInterval = 60;
            if (BO.BAS.bit_compare_or(this.TotalFlagValue, 8)) this.HoursInterval = 5;
            if (BO.BAS.bit_compare_or(this.TotalFlagValue, 16)) this.HoursInterval = 10;
            if (BO.BAS.bit_compare_or(this.TotalFlagValue, 32)) this.HoursInterval = 6;
            if (BO.BAS.bit_compare_or(this.TotalFlagValue, 64)) this.HoursInterval = 15;
            if (BO.BAS.bit_compare_or(this.TotalFlagValue, 128)) this.TimesheetEntryByMinutes = true;
            if (BO.BAS.bit_compare_or(this.TotalFlagValue, 256)) this.OfferTrimming = true;
            if (BO.BAS.bit_compare_or(this.TotalFlagValue, 512)) this.OfferContactPerson = true;
        }
    }
}

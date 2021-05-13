using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p31view
{
    public abstract class calendarBaseViewModel:BaseViewModel
    {
        public int j02ID { get; set; }
        public string SelectedPerson { get; set; }
        public bool ShowHHMM { get; set; }
        public DateTime d0 { get; set; }
        public DateTime d1 { get; set; }
        public DateTime d2 { get; set; }

        public bool ShowWeekend { get; set; }

        public IEnumerable<BO.p31Worksheet> lisP31 { get; set; }
        public IEnumerable<BO.c26Holiday> lisC26 { get; set; }
        public List<BO.p31WorksheetTimelineDay> lisSums { get; set; }
    }
}

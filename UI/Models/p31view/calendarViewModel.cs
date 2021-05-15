using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p31view
{
    public enum CalendarViewEnum
    {
        Month=1,
        MonthAgenda=2,
        Week=3,
        WeekAgenda=4,
        NAgenda=5,
        ExactDay=6
    }
    public class calendarViewModel:BaseViewModel
    {
        public int j02ID { get; set; }
        public BO.j02Person RecJ02 { get; set; }
        public bool ShowHHMM { get; set; }
        public bool ShowP31Recs { get; set; }
        public bool ShowP31RecsNoTime { get; set; }

        public CalendarViewEnum CurrentView { get; set; } = CalendarViewEnum.Month;
       

        public DateTime d0 { get; set; }
        public DateTime d1 { get; set; }
        public DateTime d2 { get; set; }
        public int m0 { get; set; }
        public int y0 { get; set; }

        public int FirstDayOfWeek = 7;

        public bool ShowWeekend { get; set; }
        public bool ShowLeftPanel { get; set; }

        public IEnumerable<BO.p31Worksheet> lisP31 { get; set; }
        public IEnumerable<BO.c26Holiday> lisC26 { get; set; }
        public List<BO.p31WorksheetTimelineDay> lisSums { get; set; }

        public string StatTotalsByPrefix { get; set; }
        
    }
}

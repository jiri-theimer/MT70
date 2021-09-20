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

        public int h0 { get; set; } = 8; //u přesného dne první hodina
        public int h1 { get; set; } = 19; //u přesného dne poslední hodina

        public int FirstDayOfWeek = 7;

        public bool ShowWeekend { get; set; }
        public bool ShowLeftPanel { get; set; }

        public IEnumerable<BO.p31Worksheet> lisP31 { get; set; }
        public IEnumerable<BO.c26Holiday> lisC26 { get; set; }
        public List<BO.p31WorksheetTimelineDay> lisSums { get; set; }

        public string StatTotalsByPrefix { get; set; }

        public bool IsAgendaDescending { get; set; }
        public int AgendaNdays { get; set; }
        public int DayView_MinutesGap { get; set; }


        public int GetMinuteGapPixels()
        {
            switch (this.DayView_MinutesGap)
            {
                case 5:
                    return 8;
                case 10:
                    return 7;
                case 15:
                    return 6;
                case 20:
                    return 5;
                case 30:
                    return 5;
                case 60:
                    return 5;
                default:
                    return 3;
            }
        }
    }
}

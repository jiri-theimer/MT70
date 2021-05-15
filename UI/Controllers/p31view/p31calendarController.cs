using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.p31view;

namespace UI.Controllers.p31view
{
    public class p31calendarController : BaseController
    {
        public IActionResult Index(int j02id,string d,int cv)
        {            
            var v = new calendarViewModel() { d0 = DateTime.Today,j02ID=j02id,CurrentView= CalendarViewEnum.Month };            
            
            handle_defaults(v,d,cv);
            

            v.lisC26 = Factory.c26HolidayBL.GetList(new BO.myQueryC26() { global_d1 = v.d1, global_d2 = v.d2 });

            v.lisSums = Factory.p31WorksheetBL.GetList_TimelineDays(new List<int> { v.j02ID }, v.d1, v.d2, 0).ToList();
            var mq = new BO.myQueryP31() {j02id=v.j02ID, global_d1 = v.d1, global_d2 = v.d2 };            
            v.lisP31 = Factory.p31WorksheetBL.GetList(mq);
            
            return View(v);
        }

        private void handle_defaults(calendarViewModel v,string d,int cv)
        {
            if (cv < 0 || cv > 6) cv = 0;            
            if (cv == 0)
            {
                cv = Factory.CBL.LoadUserParamInt("p31calendar-cv",1);
            }
            v.CurrentView = (CalendarViewEnum)cv;
            v.ShowHHMM = false;
            if (Factory.CurrentUser.j03DefaultHoursFormat == "T") v.ShowHHMM = true;
            v.StatTotalsByPrefix = Factory.CBL.LoadUserParam("p31calendar-totalsby", "p28");
            v.ShowLeftPanel = Factory.CBL.LoadUserParamBool("p31calendar-showleftpanel", true);
            v.ShowWeekend = Factory.CBL.LoadUserParamBool("p31calendar-showweekend", true);
            v.ShowP31Recs = Factory.CBL.LoadUserParamBool("p31calendar-showp31recs", true);
            v.ShowP31RecsNoTime = Factory.CBL.LoadUserParamBool("p31calendar-showp31recsnotime", true);
            v.j02ID = Factory.CBL.LoadUserParamInt("p31calendar-j02id", Factory.CurrentUser.j02ID);
            v.RecJ02 = Factory.j02PersonBL.Load(v.j02ID);

            if (!string.IsNullOrEmpty(d))
            {
                try
                {
                    v.d0 = BO.BAS.String2Date(d);
                }
                catch
                {
                    v.d0 = DateTime.Today;
                }

            }
            v.m0 = v.d0.Month;
            v.y0 = v.d0.Year;

            switch (v.CurrentView)
            {
                
                case CalendarViewEnum.Month:
                    mesic_handle_defaults(v);
                    break;
                case CalendarViewEnum.Week:
                    tyden_handle_defaults(v);
                    break;
                case CalendarViewEnum.WeekAgenda:
                    tyden_handle_defaults(v);
                    break;
                default:
                    
                    break;
                
            }
            
        }

        private void mesic_handle_defaults(calendarViewModel v)
        {
            v.d1 = new DateTime(v.d0.Year, v.d0.Month, 1);            
            v.d2 = v.d1.AddMonths(1).AddDays(-1);
            v.d1 = get_first_prev_monday(v.d1);
            v.d2 = get_first_prev_monday(v.d2).AddDays(6);


        }

        private void tyden_handle_defaults(calendarViewModel v)
        {
            v.d1 = get_first_prev_monday(v.d0);
            v.d2 = v.d1.AddDays(6);


        }



        private DateTime get_first_prev_monday(DateTime d)
        {
            for (int i = 0; i <= 7; i++)
            {
                if (d.DayOfWeek == DayOfWeek.Monday)
                {
                    return d;
                }                
                d = d.AddDays(-1);
            }
            return d;
        }

        
        

    }
}

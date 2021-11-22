using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace UI.Controllers
{
    public class iCalendarController : Controller
    {
        private readonly BL.RunningApp _app;

        
        private BL.Factory _f;

        public iCalendarController(BL.RunningApp app, BL.TheEntitiesProvider ep, BL.TheTranslator tt)
        {
           
            _app = app;

            var ru = new BO.RunningUser() { j03Login = "lamos" };
            _f = new BL.Factory(ru, _app,ep, tt);

        }

        public IActionResult p31(string pids,string p32ids,string p41ids,string d1,string d2)    //spuštění ručně přes prohlížeč
        {
            if (string.IsNullOrEmpty(pids) && string.IsNullOrEmpty(p32ids) && string.IsNullOrEmpty(p32ids) && string.IsNullOrEmpty(p41ids) && (string.IsNullOrEmpty(d1) || string.IsNullOrEmpty(d2)))
            {
                return null;
            }
            var c = new BL.bas.ICalendarGenerator(_f);
            var gd1 = new DateTime(2000, 1, 1);
            var gd2 = new DateTime(2100, 1, 1);
            if (d1 != null) gd1 = BO.BAS.String2Date(d1);
            if (d2 != null) gd2 = BO.BAS.String2Date(d2);

            string s = c.Generate_p31_Calendar(pids, p32ids, p41ids, gd1, gd2);

            //Response.Headers["Content-Disposition"] = "inline; filename=marktime_calendar.ics";
            Response.Headers["Content-Disposition"] = $"filename=marktime_p31_calendar.ics";
            var fileContentResult = new FileContentResult(System.Text.Encoding.UTF8.GetBytes(s), "text/calendar");
            return fileContentResult;

            //var calendarBytes = System.Text.Encoding.UTF8.GetBytes(s);
            //MemoryStream ms = new MemoryStream(calendarBytes);

            //System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(ms, "event.ics", "text/calendar");

        }

        public IActionResult p56(string pids, string p57ids, string p41ids, string d1, string d2)    //spuštění ručně přes prohlížeč
        {
            if (string.IsNullOrEmpty(pids) && string.IsNullOrEmpty(p57ids) && string.IsNullOrEmpty(p41ids) && (string.IsNullOrEmpty(d1) || string.IsNullOrEmpty(d2)))
            {
                return null;
            }
            var c = new BL.bas.ICalendarGenerator(_f);
            var gd1 = new DateTime(2000, 1, 1);
            var gd2 = new DateTime(2100, 1, 1);
            if (d1 != null) gd1 = BO.BAS.String2Date(d1);
            if (d2 != null) gd2 = BO.BAS.String2Date(d2);

            string s = c.Generate_p56_Calendar(pids,null,p57ids,"", gd1, gd2);
        
            Response.Headers["Content-Disposition"] = $"filename=marktime_p56_calendar.ics";
            var fileContentResult = new FileContentResult(System.Text.Encoding.UTF8.GetBytes(s), "text/calendar");
            return fileContentResult;

            

        }
    }
}

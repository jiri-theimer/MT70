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
        public IActionResult Mesic(int j02id,string d)
        {
            var v = new calmonthViewModel() { d0 = DateTime.Today,j02ID=j02id };            
            if (!string.IsNullOrEmpty(d))
            {
                v.d0 = BO.BAS.String2Date(d);
            }
            handle_defaults(v);
            mesic_handle_defaults(v);

            v.lisC26 = Factory.c26HolidayBL.GetList(new BO.myQueryC26() { global_d1 = v.d1, global_d2 = v.d2 });

            v.lisSums = Factory.p31WorksheetBL.GetList_TimelineDays(new List<int> { v.j02ID }, v.d1, v.d2, 0).ToList();
            var mq = new BO.myQueryP31() {j02id=v.j02ID, global_d1 = v.d1, global_d2 = v.d2 };            
            v.lisP31 = Factory.p31WorksheetBL.GetList(mq);
            
            return View(v);
        }

        private void handle_defaults(calendarBaseViewModel v)
        {
            v.ShowHHMM = false;
            if (Factory.CurrentUser.j03DefaultHoursFormat == "T") v.ShowHHMM = true;
            v.j02ID = Factory.CBL.LoadUserParamInt("p31calendar-j02id", Factory.CurrentUser.j02ID);
            
            v.RecJ02 = Factory.j02PersonBL.Load(v.j02ID);
        }

        private void mesic_handle_defaults(calmonthViewModel v)
        {
            v.d1 = new DateTime(v.d0.Year, v.d0.Month, 1);
            v.d2 = v.d1.AddMonths(1).AddDays(-1);
            

        }

        
    }
}

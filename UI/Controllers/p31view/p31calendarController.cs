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
            if (v.j02ID == 0) v.j02ID = Factory.CurrentUser.j02ID;
            if (!string.IsNullOrEmpty(d))
            {
                v.d0 = BO.BAS.String2Date(d);
            }

            mesic_handle_defaults(v);



            v.lisSums = Factory.p31WorksheetBL.GetList_TimelineDays(new List<int> { v.j02ID }, v.d1, v.d2, 0).ToList();
            var mq = new BO.myQueryP31() { global_d1 = v.d1, global_d2 = v.d2 };
            v.lisC26 = Factory.c26HolidayBL.GetList(new BO.myQueryC26() { global_d1 = v.d1, global_d2 = v.d2 });

            v.lisP31 = Factory.p31WorksheetBL.GetList(mq);
            
            return View(v);
        }

        private void mesic_handle_defaults(calmonthViewModel v)
        {
            v.d1 = new DateTime(v.d0.Year, v.d0.Month, 1);
            v.d2 = v.d1.AddMonths(1).AddDays(-1);

           
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.p31view;

namespace UI.Controllers
{
    public class p31viewController : BaseController
    {
        //DAYLINE rozhraní
        public IActionResult dayline()
        {
            var v = new daylineViewModel() { d0 = DateTime.Today };
            v.d1 = new DateTime(v.d0.Year, v.d0.Month, 1);
            v.d2 = v.d1.AddMonths(1).AddDays(-1);



            v.lisSums = Factory.p31WorksheetBL.GetList_TimelineDays(getJ02IDs(v), v.d1, v.d2,0);
            var mq = new BO.myQueryP31() { global_d1 = v.d1, global_d2 = v.d2 };           
            v.lisP31 = Factory.p31WorksheetBL.GetList(mq);

            return View(v);
        }


        public List<int> getJ02IDs(daylineViewModel v)
        {
            return null;
        }
    }
}

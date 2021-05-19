using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Controllers
{
    public class PeriodController : BaseController
    {
        private readonly BL.ThePeriodProvider _pp;
        public PeriodController(BL.ThePeriodProvider pp)
        {
            _pp = pp;
        }

        public IActionResult PeriodIFrame(string prefix,string userparamkey)
        {
            var v = new PeriodViewModel() { prefix = prefix };
            if (!string.IsNullOrEmpty(userparamkey))    //default je hodnota: grid-period
            {
                v.UserParamKey = userparamkey;
            }

            v.InhaleUserPeriodSetting(_pp,Factory);

            
            return View(v);
        }



        public IActionResult UserPeriods()
        {
            var v = new UserPeriodsViewModel();

            v.lisX21 = Factory.FBL.GetListX21(Factory.CurrentUser.pid).ToList();
            foreach(var c in v.lisX21)
            {
                c.TempGuid = BO.BAS.GetGuid();
            }

            return View(v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserPeriods(UserPeriodsViewModel v, string oper, string guid)
        {
            
            if (oper == "postback")
            {
                return View(v);
            }
            
            if (oper == "add")
            {
                var c = new BO.x21DatePeriod() { TempGuid = BO.BAS.GetGuid(),x21ValidFrom=DateTime.Today,x21ValidUntil=DateTime.Today.AddDays(30) };               
                v.lisX21.Add(c);
                return View(v);
            }
            if (oper == "delete")
            {
                v.lisX21.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
           
            

            if (ModelState.IsValid)
            {
                

                
                
                if (Factory.FBL.SaveX21Batch(v.lisX21))
                {
                    
                    v.SetJavascript_CallOnLoad(0);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}

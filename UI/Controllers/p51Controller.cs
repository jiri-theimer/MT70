using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class p51Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new p51Record() { rec_pid = pid, rec_entity = "p51" };
            v.Rec = new BO.p51PriceList();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p51PriceListBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboJ27Code = v.Rec.j27Code;
               
                


            }
           
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

        private void RefreshState(p51Record v)
        {
            

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p51Record v, string oper)
        {
            RefreshState(v);

            if (oper == "postback")
            {
                return View(v);
            }

            if (ModelState.IsValid)
            {
                BO.p51PriceList c = new BO.p51PriceList();
                if (v.rec_pid > 0) c = Factory.p51PriceListBL.Load(v.rec_pid);

                c.p51TypeFlag = v.Rec.p51TypeFlag;
                c.p51Name = v.Rec.p51Name;
                c.j27ID = v.Rec.j27ID;
                c.p51DefaultRateT = v.Rec.p51DefaultRateT;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.p51PriceListBL.Save(c, v.lisP52);

                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}

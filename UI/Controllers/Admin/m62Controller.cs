using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;


namespace UI.Controllers.Admin
{
    public class m62Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new m62Record() { rec_pid = pid, rec_entity = "m62" };
            v.Rec = new BO.m62ExchangeRate();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.m62ExchangeRateBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboJ27Master = v.Rec.j27Code_Master;
                v.ComboJ27Slave = v.Rec.j27Code_Slave;

            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

        private void RefreshState(m62Record v)
        {


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(m62Record v)
        {
            RefreshState(v);

            if (ModelState.IsValid)
            {
                BO.m62ExchangeRate c = new BO.m62ExchangeRate();
                if (v.rec_pid > 0) c = Factory.m62ExchangeRateBL.Load(v.rec_pid);
                c.m62Date = v.Rec.m62Date;
                c.m62RateType = v.Rec.m62RateType;
                c.m62Units = v.Rec.m62Units;
                c.m62Rate = v.Rec.m62Rate;
                c.j27ID_Master = v.Rec.j27ID_Master;
                c.j27ID_Slave = v.Rec.j27ID_Slave;
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.m62ExchangeRateBL.Save(c);
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

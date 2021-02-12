using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class j07Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j07Record() { rec_pid = pid, rec_entity = "j07" };
            v.Rec = new BO.j07PersonPosition();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j07PersonPositionBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                

            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

        private void RefreshState(j07Record v)
        {
            v.lisP87 = Factory.FBL.GetListP87();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(j07Record v)
        {
            RefreshState(v);

            if (ModelState.IsValid)
            {
                BO.j07PersonPosition c = new BO.j07PersonPosition();
                if (v.rec_pid > 0) c = Factory.j07PersonPositionBL.Load(v.rec_pid);
                c.j07Name = v.Rec.j07Name;
                c.j07FreeText01 = v.Rec.j07FreeText01;
                c.j07FreeText02 = v.Rec.j07FreeText02;
                c.j07Ordinary = v.Rec.j07Ordinary;
                c.j07Name_BillingLang1 = v.Rec.j07Name_BillingLang1;
                c.j07Name_BillingLang2 = v.Rec.j07Name_BillingLang2;
                c.j07Name_BillingLang3 = v.Rec.j07Name_BillingLang3;
                c.j07Name_BillingLang4 = v.Rec.j07Name_BillingLang4;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.j07PersonPositionBL.Save(c);
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

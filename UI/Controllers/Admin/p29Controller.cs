using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class p29Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new p29Record() { rec_pid = pid, rec_entity = "p29" };
            v.Rec = new BO.p29ContactType();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p29ContactTypeBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboB01Name = v.Rec.b01Name;
                v.ComboX38Name = v.Rec.x38Name;

            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

        private void RefreshState(p29Record v)
        {


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p29Record v)
        {
            RefreshState(v);

            if (ModelState.IsValid)
            {
                BO.p29ContactType c = new BO.p29ContactType();
                if (v.rec_pid > 0) c = Factory.p29ContactTypeBL.Load(v.rec_pid);
                c.p29Name = v.Rec.p29Name;
                c.x38ID = v.Rec.x38ID;
                c.b01ID = v.Rec.b01ID;
                c.p29Ordinary = v.Rec.p29Ordinary;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.p29ContactTypeBL.Save(c);
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

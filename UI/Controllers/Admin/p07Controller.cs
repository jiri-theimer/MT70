using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class p07Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new p07Record() { rec_pid = pid,rec_entity= "p07" };
            v.Rec = new BO.p07ProjectLevel();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p07ProjectLevelBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            
            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p07Record v)
        {

            if (ModelState.IsValid)
            {
                BO.p07ProjectLevel c = new BO.p07ProjectLevel();
                if (v.rec_pid > 0) c = Factory.p07ProjectLevelBL.Load(v.rec_pid);
                c.p07Level = v.Rec.p07Level;
                c.p07NameSingular = v.Rec.p07NameSingular;
                c.p07NamePlural = v.Rec.p07NamePlural;
                c.p07NameInflection = v.Rec.p07NameInflection;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.p07ProjectLevelBL.Save(c);
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

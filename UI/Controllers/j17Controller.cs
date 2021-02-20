using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class j17Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j17Record() { rec_pid = pid, rec_entity = "j17" };
            v.Rec = new BO.j17Country();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j17CountryBL.Load(v.rec_pid);
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

        private void RefreshState(j17Record v)
        {


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(j17Record v)
        {
            RefreshState(v);

            if (ModelState.IsValid)
            {
                BO.j17Country c = new BO.j17Country();
                if (v.rec_pid > 0) c = Factory.j17CountryBL.Load(v.rec_pid);
                c.j17Name = v.Rec.j17Name;
                c.j17Code = v.Rec.j17Code;
                c.j17Ordinary = v.Rec.j17Ordinary;
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.j17CountryBL.Save(c);
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

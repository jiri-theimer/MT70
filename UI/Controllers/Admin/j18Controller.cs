using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class j18Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j18Record() { rec_pid = pid, rec_entity = "j18" };
            v.Rec = new BO.j18Region();
            v.roles = new RoleAssignViewModel() { RecPid = v.rec_pid, RecPrefix = "j18" };
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j18RegionBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboJ17Name = v.Rec.j17Name;

            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

        private void RefreshState(j18Record v)
        {
            

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(j18Record v)
        {
            RefreshState(v);

            if (ModelState.IsValid)
            {
                BO.j18Region c = new BO.j18Region();
                if (v.rec_pid > 0) c = Factory.j18RegionBL.Load(v.rec_pid);
                c.j18Name = v.Rec.j18Name;
                c.j18Code = v.Rec.j18Code;
                c.j18Ordinary = v.Rec.j18Ordinary;
                c.j17ID = v.Rec.j17ID;
               
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.j18RegionBL.Save(c, v.roles.getList4Save(Factory));
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

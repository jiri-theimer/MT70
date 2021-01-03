using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class p42Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new p42Record() { rec_pid = pid, rec_entity = "p42" };
            v.Rec = new BO.p42ProjectType();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p42ProjectTypeBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboB01Name = v.Rec.b01Name;
                v.ComboP07Name = v.Rec.p07Name;
                v.ComboX38Name = v.Rec.x38Name;
                v.SelectedP34IDs = Factory.p34ActivityGroupBL.GetList(new BO.myQueryP34() { p42id = v.rec_pid }).Select(p => p.pid).ToList();
                
                

            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

        private void RefreshState(p42Record v)
        {
            v.lisAllP34 = Factory.p34ActivityGroupBL.GetList(new BO.myQueryP34());

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p42Record v)
        {
            RefreshState(v);
            
            if (ModelState.IsValid)
            {
                BO.p42ProjectType c = new BO.p42ProjectType();
                if (v.rec_pid > 0) c = Factory.p42ProjectTypeBL.Load(v.rec_pid);
                c.b01ID = v.Rec.b01ID;
                c.x38ID = v.Rec.x38ID;
                c.p07ID = v.Rec.p07ID;
                c.p42Ordinary = v.Rec.p42Ordinary;
                c.p42Name = v.Rec.p42Name;
                c.p42Code = v.Rec.p42Code;
                c.p42ArchiveFlag = v.Rec.p42ArchiveFlag;
                c.p42ArchiveFlagP31 = v.Rec.p42ArchiveFlagP31;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.p42ProjectTypeBL.Save(c,v.SelectedP34IDs);
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

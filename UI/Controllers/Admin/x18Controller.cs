using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class x18Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new x18Record() { rec_pid = pid, rec_entity = "x18" };
            v.Rec = new BO.x18EntityCategory();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x18EntityCategoryBL.Load(v.rec_pid);
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
        public IActionResult Record(Models.Record.x18Record v)
        {

            if (ModelState.IsValid)
            {
                BO.x18EntityCategory c = new BO.x18EntityCategory();
                if (v.rec_pid > 0) c = Factory.x18EntityCategoryBL.Load(v.rec_pid);
                c.x18Name = v.Rec.x18Name;
                c.x18NameShort = v.Rec.x18NameShort;
                c.x18Ordinary = v.Rec.x18Ordinary;
                c.x18EntryCodeFlag = v.Rec.x18EntryCodeFlag;
                c.x18EntryNameFlag = v.Rec.x18EntryNameFlag;
                c.x18UploadFlag = v.Rec.x18UploadFlag;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                var lisX16 = new List<BO.x16EntityCategory_FieldSetting>();
                var lisX20 = new List<BO.x20EntiyToCategory>();
                
                c.pid = Factory.x18EntityCategoryBL.Save(c,lisX20,lisX16);
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

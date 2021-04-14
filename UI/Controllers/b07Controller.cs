using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class b07Controller : BaseController
    {
        public IActionResult List(string master_entity,int master_pid)
        {
            if (master_entity == null || master_pid == 0)
            {
                return this.StopPageSubform("master_entity or master_pid missing");
            }
            var v = new b07list() { master_entity = master_entity, master_pid = master_pid };
            v.lisB07 = Factory.b07CommentBL.GetList(new BO.myQueryB07() { x29id =BO.BASX29.GetInt(v.master_entity.Substring(0,3)), recpid = v.master_pid });
            return View(v);
        }
        public IActionResult Record(int pid, bool isclone,string prefix,int recpid)
        {
            var v = new b07Record() { rec_pid = pid, rec_entity = "b07",prefix=prefix,recpid=recpid, UploadGuid=BO.BAS.GetGuid() };
            v.Rec = new BO.b07Comment();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.b07CommentBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.prefix = BO.BASX29.GetPrefix(v.Rec.x29ID);
                v.recpid = v.Rec.b07RecordPID;
            }
            if (v.Rec.pid==0 && (string.IsNullOrEmpty(v.prefix) || v.recpid == 0))
            {
                return this.StopPage(true, "Na vstupu chybí záznam entity.");
            }
            v.ObjectAlias = Factory.CBL.GetObjectAlias(v.prefix, v.recpid);

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.b07Record v)
        {

            if (ModelState.IsValid)
            {
                BO.b07Comment c = new BO.b07Comment();
                if (v.rec_pid > 0) c = Factory.b07CommentBL.Load(v.rec_pid);
                c.x29ID = BO.BASX29.GetEnum(v.prefix);
                c.b07RecordPID = v.recpid;
                c.b07Value = v.Rec.b07Value;
                c.b07Date = v.Rec.b07Date;
                c.b07ReminderDate = v.Rec.b07ReminderDate;
                c.b07LinkName = v.Rec.b07LinkName;
                c.b07LinkUrl = v.Rec.b07LinkUrl;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.b07CommentBL.Save(c,v.UploadGuid);
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

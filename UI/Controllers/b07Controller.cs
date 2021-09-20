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
            var mq = new BO.myQueryB07();
            if (master_entity.Substring(0, 2) == "le")
            {
                if (master_entity == "le5")
                {
                    mq.x29id = 141;
                    mq.recpid = v.master_pid;
                }
                else
                {
                    mq.leindex = Convert.ToInt32(master_entity.Substring(2, 1));
                    mq.lepid = v.master_pid;
                }
                
            }
            else
            {
                mq.x29id = BO.BASX29.GetInt(v.master_entity.Substring(0, 3));
                mq.recpid = v.master_pid;
            }
            v.lisB07 = Factory.b07CommentBL.GetList(mq).Where(p => p.b07WorkflowInfo == null && p.b07Value !="upload");
            

            v.lisO27 = Factory.o27AttachmentBL.GetList(new BO.myQueryO27() { b07ids = v.lisB07.Select(p => p.pid).ToList() });
            return View(v);
        }
        public IActionResult Record(int pid, bool isclone,string prefix,int recpid)
        {
            var v = new b07Record() { rec_pid = pid, rec_entity = "b07",recprefix=BO.BASX29.GetPrefixDb(prefix),recpid=recpid, UploadGuid=BO.BAS.GetGuid() };
            v.Rec = new BO.b07Comment();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.b07CommentBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.recprefix = BO.BASX29.GetPrefix(v.Rec.x29ID);
                v.recpid = v.Rec.b07RecordPID;

                var lisO27 = Factory.o27AttachmentBL.GetList(new BO.myQueryO27() { b07id = v.rec_pid });
                v.lisO27 = new List<o27Repeator>();
                foreach (var c in lisO27)
                {
                    v.lisO27.Add(new o27Repeator()
                    {
                        TempGuid = BO.BAS.GetGuid(),
                        pid = c.pid,
                        o27OriginalFileName = c.o27OriginalFileName,
                        o27FileSize=c.o27FileSize,
                        o27GUID =c.o27GUID                        
                    });
                }

            }
            if (v.Rec.pid==0 && (string.IsNullOrEmpty(v.recprefix) || v.recpid == 0))
            {
                return this.StopPage(true, "Na vstupu chybí záznam entity.");
            }
            v.ObjectAlias = Factory.CBL.GetObjectAlias(v.recprefix, v.recpid);
            

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            RefreshState(v);
            return View(v);
        }

        private void RefreshState(b07Record v)
        {
            if (v.lisO27 == null)
            {
                v.lisO27 = new List<o27Repeator>();
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.b07Record v,string oper,string guid)
        {
            RefreshState(v);
            if (oper== "delete_o27")
            {
                v.lisO27.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (ModelState.IsValid)
            {                
                BO.b07Comment c = new BO.b07Comment();
                if (v.rec_pid > 0) c = Factory.b07CommentBL.Load(v.rec_pid);
                c.x29ID = BO.BASX29.GetEnum(v.recprefix);
                c.b07RecordPID = v.recpid;
                c.b07Value = v.Rec.b07Value;
                c.b07Date = v.Rec.b07Date;
                c.b07ReminderDate = v.Rec.b07ReminderDate;
                c.b07LinkName = v.Rec.b07LinkName;
                c.b07LinkUrl = v.Rec.b07LinkUrl;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

               
                c.pid = Factory.b07CommentBL.Save(c,v.UploadGuid, v.lisO27.Where(p => p.IsTempDeleted).Select(p => p.pid).ToList());
                if (c.pid > 0)
                {

                    if (v.recpid > 0)
                    {
                        v.SetJavascript_CallOnLoad(v.recpid);
                    }
                    else
                    {
                        v.SetJavascript_CallOnLoad(c.pid);
                    }
                    
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}

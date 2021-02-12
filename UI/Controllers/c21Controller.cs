using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class c21Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new c21Record() { rec_pid = pid, rec_entity = "c21" };
            v.Rec = new BO.c21FondCalendar();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.c21FondCalendarBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

                v.lisC28 = Factory.c21FondCalendarBL.GetList_c28(v.rec_pid).ToList();
                foreach (var c in v.lisC28)
                {
                    c.TempGuid = BO.BAS.GetGuid();
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

        private void RefreshState(c21Record v)
        {
            if (v.lisC28 == null)
            {
                v.lisC28 = new List<BO.c28FondCalendar_Log>();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(c21Record v,string oper, string guid)
        {
            RefreshState(v);

            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "add_c28")
            {
                var c = new BO.c28FondCalendar_Log() { TempGuid = BO.BAS.GetGuid() };
                v.lisC28.Add(c);
                return View(v);
            }
            if (oper == "delete_c28")
            {
                v.lisC28.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }

            if (ModelState.IsValid)
            {
                BO.c21FondCalendar c = new BO.c21FondCalendar();
                if (v.rec_pid > 0) c = Factory.c21FondCalendarBL.Load(v.rec_pid);
                c.c21Name = v.Rec.c21Name;
                c.c21ScopeFlag = v.Rec.c21ScopeFlag;
                c.c21Ordinary = v.Rec.c21Ordinary;
                c.c21Day1_Hours = v.Rec.c21Day1_Hours;
                c.c21Day2_Hours = v.Rec.c21Day2_Hours;
                c.c21Day3_Hours = v.Rec.c21Day3_Hours;
                c.c21Day4_Hours = v.Rec.c21Day4_Hours;
                c.c21Day5_Hours = v.Rec.c21Day5_Hours;
                c.c21Day6_Hours = v.Rec.c21Day6_Hours;
                c.c21Day7_Hours = v.Rec.c21Day7_Hours;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.c21FondCalendarBL.Save(c,v.lisC28.Where(p=>p.IsTempDeleted==false).ToList());
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

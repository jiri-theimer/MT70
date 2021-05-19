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
        public IActionResult ProjectLevels()
        {
            var v = new p07ProjectLevelsViewModel();
            v.RecL1 = Factory.p07ProjectLevelBL.LoadByLevel(1);
            if (v.RecL1 == null) v.RecL1 = new BO.p07ProjectLevel() { p07Level = 1 };
            v.UseL1 = !v.RecL1.isclosed;

            v.RecL2 = Factory.p07ProjectLevelBL.LoadByLevel(2);
            if (v.RecL2 == null) v.RecL2 = new BO.p07ProjectLevel() { p07Level = 2 };
            v.UseL2 = !v.RecL2.isclosed;

            v.RecL3 = Factory.p07ProjectLevelBL.LoadByLevel(3);
            if (v.RecL3 == null) v.RecL3= new BO.p07ProjectLevel() { p07Level = 3 };
            v.UseL3 = !v.RecL3.isclosed;


            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProjectLevels(p07ProjectLevelsViewModel v,string oper)
        {
            if (!string.IsNullOrEmpty(oper))
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                bool b = true;
                var c = new BO.p07ProjectLevel();
                if (v.RecL1.pid > 0) c = Factory.p07ProjectLevelBL.Load(v.RecL1.pid);
                c.p07Level = 1;
                
                if (v.UseL1)
                {
                    c.p07NameSingular = v.RecL1.p07NameSingular;
                    c.p07NamePlural = v.RecL1.p07NamePlural;
                    c.p07NameInflection = v.RecL1.p07NameInflection;
                    c.ValidUntil = new DateTime(3000, 1, 1);
                }
                else
                {
                    c.ValidUntil = DateTime.Now;
                }

                c.pid = Factory.p07ProjectLevelBL.Save(c);
                if (c.pid == 0)
                {
                    b = false;
                    this.Notify_RecNotSaved();                    
                }

                c = new BO.p07ProjectLevel();
                if (v.RecL2.pid > 0) c = Factory.p07ProjectLevelBL.Load(v.RecL2.pid);
                c.p07Level = 2;
                
                if (v.UseL2)
                {
                    c.p07NameSingular = v.RecL2.p07NameSingular;
                    c.p07NamePlural = v.RecL2.p07NamePlural;
                    c.p07NameInflection = v.RecL2.p07NameInflection;
                    c.ValidUntil = new DateTime(3000, 1, 1);
                }
                else
                {
                    c.ValidUntil = DateTime.Now;
                }

                c.pid = Factory.p07ProjectLevelBL.Save(c);
                if (c.pid == 0)
                {
                    b = false;
                    this.Notify_RecNotSaved();
                }

                c = new BO.p07ProjectLevel();
                if (v.RecL3.pid > 0) c = Factory.p07ProjectLevelBL.Load(v.RecL3.pid);
                c.p07Level = 3;
                c.p07NameSingular = v.RecL3.p07NameSingular;
                c.p07NamePlural = v.RecL3.p07NamePlural;
                c.p07NameInflection = v.RecL3.p07NameInflection;


                c.pid = Factory.p07ProjectLevelBL.Save(c);
                if (c.pid == 0)
                {
                    b = false;
                    this.Notify_RecNotSaved();
                }

                if (b)
                {
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }
                

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        //public IActionResult Record(int pid, bool isclone)
        //{
        //    var v = new p07Record() { rec_pid = pid,rec_entity= "p07" };
        //    v.Rec = new BO.p07ProjectLevel();
        //    if (v.rec_pid > 0)
        //    {
        //        v.Rec = Factory.p07ProjectLevelBL.Load(v.rec_pid);
        //        if (v.Rec == null)
        //        {
        //            return RecNotFound(v);
        //        }

        //    }
        //    v.Toolbar = new MyToolbarViewModel(v.Rec);
        //    if (isclone)
        //    {
        //        v.MakeClone();
        //    }
            
        //    return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Record(p07Record v)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        BO.p07ProjectLevel c = new BO.p07ProjectLevel();
        //        if (v.rec_pid > 0) c = Factory.p07ProjectLevelBL.Load(v.rec_pid);
        //        c.p07Level = v.Rec.p07Level;
        //        c.p07NameSingular = v.Rec.p07NameSingular;
        //        c.p07NamePlural = v.Rec.p07NamePlural;
        //        c.p07NameInflection = v.Rec.p07NameInflection;

        //        c.ValidUntil = v.Toolbar.GetValidUntil(c);
        //        c.ValidFrom = v.Toolbar.GetValidFrom(c);

        //        c.pid = Factory.p07ProjectLevelBL.Save(c);
        //        if (c.pid > 0)
        //        {

        //            v.SetJavascript_CallOnLoad(c.pid);
        //            return View(v);
        //        }

        //    }


        //    this.Notify_RecNotSaved();
        //    return View(v);
        //}
    }
}

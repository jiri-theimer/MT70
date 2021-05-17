using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;


namespace UI.Controllers
{
    public class x97Controller : BaseController
    {
       
        public IActionResult Record(int pid, bool isclone, string viewurl, string source, string pagetitle)
        {
            var v = new x97Record() { rec_pid = pid, rec_entity = "x97" };
            v.Rec = new BO.x97Translate();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x97TranslateBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                
            }
            
            v.Toolbar = new MyToolbarViewModel(v.Rec,false);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v,BO.x53PermValEnum.GR_Admin);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.x97Record v,string oper)
        {
            if (oper == "recovery")
            {
                Factory.Translator.Recovery();
                this.AddMessageTranslated("Překlad načten.","info");
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.x97Translate c = new BO.x97Translate();
                if (v.rec_pid > 0) c = Factory.x97TranslateBL.Load(v.rec_pid);
                c.x97Code = v.Rec.x97Code;
                c.x97Orig = v.Rec.x97Code;
                c.x97Lang1 = v.Rec.x97Lang1;
                c.x97Lang2= v.Rec.x97Lang2;
                c.x97Lang3 = v.Rec.x97Lang3;
                c.x97Lang4 = v.Rec.x97Lang4;
                c.x97OrigSource = v.Rec.x97OrigSource;

                c.pid = Factory.x97TranslateBL.Save(c);
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
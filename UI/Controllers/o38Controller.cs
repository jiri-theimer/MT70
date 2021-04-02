using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class o38Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone,int p28id,int o36id,string tempguid)
        {
            var v = new o38Record() { rec_pid = pid, rec_entity = "o38",p28ID=p28id,o36ID=o36id,TempGuid=tempguid };
            if (v.rec_pid==0 && string.IsNullOrEmpty(v.TempGuid) && v.p28ID == 0)
            {
                return StopPage(true, "Na vstupu chybí pid nebo tempguid nebo p28id");
            }
            
            v.Rec = new BO.o38Address();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.o38AddressBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                if (v.o36ID == 0)
                {
                    v.o36ID = BO.BAS.InInt(v.Rec.o38Description);
                }
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.o38Record v)
        {

            if (ModelState.IsValid)
            {
                BO.o38Address c = new BO.o38Address();
                if (v.rec_pid > 0) c = Factory.o38AddressBL.Load(v.rec_pid);
                c.o38Name = v.Rec.o38Name;
                c.o38City = v.Rec.o38City;
                c.o38Street = v.Rec.o38Street;
                c.o38ZIP = v.Rec.o38ZIP;
                c.o38Country = v.Rec.o38Country;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.o38AddressBL.Save(c,v.p28ID,v.o36ID,v.TempGuid);
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

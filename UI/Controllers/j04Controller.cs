using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class j04Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone, string prefix)
        {
            var v = new j04Record() { rec_pid = pid, rec_entity = "j04" };
            v.Rec = new BO.j04UserRole();

            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j04UserRoleBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

                v.SelectedX53IDs = Factory.j04UserRoleBL.GetList_BoundX53(v.rec_pid).Select(p => p.x53ID).ToList();
                
            }
           
           
            v.lisAllX53 = Factory.FBL.GetListX53().Where(p => p.x29ID==BO.x29IdEnum.j03User);
            


            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            return ViewTup(v, BO.x53PermValEnum.GR_Admin);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(j04Record v)
        {

            if (ModelState.IsValid)
            {
                BO.j04UserRole c = new BO.j04UserRole();
                if (v.rec_pid > 0) c = Factory.j04UserRoleBL.Load(v.rec_pid);
                
                c.j04Name = v.Rec.j04Name;
                c.j04IsMenu_Worksheet = v.Rec.j04IsMenu_Worksheet;
                c.j04IsMenu_Project = v.Rec.j04IsMenu_Project;
                c.j04IsMenu_Contact = v.Rec.j04IsMenu_Contact;
                c.j04IsMenu_People = v.Rec.j04IsMenu_People;
                c.j04IsMenu_Report = v.Rec.j04IsMenu_Report;
                c.j04IsMenu_Invoice = v.Rec.j04IsMenu_Invoice;
                c.j04IsMenu_Proforma = v.Rec.j04IsMenu_Proforma;
                c.j04IsMenu_Notepad = v.Rec.j04IsMenu_Notepad;


                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.j04UserRoleBL.Save(c, v.SelectedX53IDs);
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


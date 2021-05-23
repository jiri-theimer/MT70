using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class j61Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone,string prefix)
        {
            var v = new j61Record() { rec_pid = pid, rec_entity = "j61" };
            v.Rec = new BO.j61TextTemplate();
            if (prefix != null)
            {
                v.Rec.x29ID = BO.BASX29.GetEnum(prefix);
            }
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j61TextTemplateBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboOwner = v.Rec.Owner;

            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }

            return View(v);
        }

        private void RefreshState(j61Record v)
        {


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(j61Record v)
        {
            RefreshState(v);

            if (ModelState.IsValid)
            {
                BO.j61TextTemplate c = new BO.j61TextTemplate();
                if (v.rec_pid > 0) c = Factory.j61TextTemplateBL.Load(v.rec_pid);
                c.j02ID_Owner = v.Rec.j02ID_Owner;
                c.j61Name = v.Rec.j61Name;
                c.j61Ordinary = v.Rec.j61Ordinary;
                c.x29ID = v.Rec.x29ID;
                c.j61MailSubject = v.Rec.j61MailSubject;
                c.j61PlainTextBody = v.Rec.j61PlainTextBody;
                c.j61MailTO = v.Rec.j61MailTO;
                c.j61MailCC = v.Rec.j61MailCC;
                c.j61MailBCC = v.Rec.j61MailBCC;
                c.j61HtmlTemplateFile = v.Rec.j61HtmlTemplateFile;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);


                c.pid = Factory.j61TextTemplateBL.Save(c);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class o40Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new o40Record() { rec_pid = pid, rec_entity = "o40" };
            v.Rec = new BO.o40SmtpAccount();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.o40SmtpAccountBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.Rec.o40Password = null;
                if (v.Rec.o40SslModeFlag > 0)
                {
                    v.IsUseSSL = true;
                }


            }
            else
            {
                v.Rec.j02ID_Owner = Factory.CurrentUser.j02ID;
            }
            if (v.Rec.j02ID_Owner > 0)
            {
                v.ComboPerson = Factory.j02PersonBL.Load(v.Rec.j02ID_Owner).FullNameDesc;
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
        public IActionResult Record(o40Record v)
        {
            if (ModelState.IsValid)
            {
                BO.o40SmtpAccount c = new BO.o40SmtpAccount();
                if (v.rec_pid > 0) c = Factory.o40SmtpAccountBL.Load(v.rec_pid);
                c.j02ID_Owner = v.Rec.j02ID_Owner;
                c.o40IsGlobalDefault = v.Rec.o40IsGlobalDefault;
                c.o40Name = v.Rec.o40Name;
                c.o40Server = v.Rec.o40Server;
                c.o40Port = v.Rec.o40Port;                
                c.o40EmailAddress = v.Rec.o40EmailAddress;
                c.o40IsUsePersonalReply = v.Rec.o40IsUsePersonalReply;
                c.o40Login = v.Rec.o40Login;
                if (String.IsNullOrEmpty(v.Rec.o40Password) == false)
                {
                    c.o40Password = v.Rec.o40Password;
                }

                c.o40IsVerify = v.Rec.o40IsVerify;
                if (v.IsUseSSL)
                {
                    c.o40SslModeFlag = BO.SslModeENUM.Explicit;
                }
                else
                {
                    c.o40SslModeFlag = BO.SslModeENUM._NoSSL;
                }
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid= Factory.o40SmtpAccountBL.Save(c);
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
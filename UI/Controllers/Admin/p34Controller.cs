using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class p34Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new p34Record() { rec_pid = pid, rec_entity = "p34" };
            v.Rec = new BO.p34ActivityGroup();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p34ActivityGroupBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

                if (v.Rec.p34ValueOffFlag > 0)
                {
                    v.IsOffBilling = true;
                }
                if (v.Rec.p34ActivityEntryFlag == BO.p34ActivityEntryFlagENUM.AktivitaSeNezadava)
                {
                    var lisP32 = Factory.p32ActivityBL.GetList(new BO.myQueryP32() { p34id = v.rec_pid }).Where(p => p.p32IsSystemDefault == true);
                    if (lisP32.Count() > 0)
                    {
                        v.p32ID = lisP32.First().pid;
                        v.ComboP32Name = lisP32.First().p32Name;
                    }
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

        private void RefreshState(p34Record v)
        {
            v.lisP87 = Factory.FBL.GetListP87();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p34Record v,string oper)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.p34ActivityGroup c = new BO.p34ActivityGroup();
                if (v.rec_pid > 0) c = Factory.p34ActivityGroupBL.Load(v.rec_pid);
                c.p33ID = v.Rec.p33ID;
                c.p34ActivityEntryFlag = v.Rec.p34ActivityEntryFlag;
                c.p34IncomeStatementFlag = v.Rec.p34IncomeStatementFlag;                
                c.p34Name = v.Rec.p34Name;
                if (v.IsOffBilling)
                {
                    c.p34ValueOffFlag = 1;
                }
                else
                {
                    c.p34ValueOffFlag = 0;
                }
                
                c.p34Code = v.Rec.p34Code;
                c.p34Ordinary = v.Rec.p34Ordinary;
                c.p34Name_BillingLang1 = v.Rec.p34Name_BillingLang1;
                c.p34Name_BillingLang2 = v.Rec.p34Name_BillingLang2;
                c.p34Name_BillingLang3 = v.Rec.p34Name_BillingLang3;
                c.p34Name_BillingLang4 = v.Rec.p34Name_BillingLang4;
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.p34ActivityGroupBL.Save(c,v.p32ID);
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

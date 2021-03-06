﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers.Admin
{
    public class p89Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new p89Record() { rec_pid = pid, rec_entity = "p89" };
            v.Rec = new BO.p89ProformaType();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p89InvoiceTypeBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboP93Name = v.Rec.p93Name;
                if (v.Rec.x31ID > 0)
                {
                    v.ComboX31Name = Factory.x31ReportBL.Load(v.Rec.x31ID).x31Name;
                }
                if (v.Rec.x31ID_Payment > 0)
                {
                    v.ComboX31Name_Payment = Factory.x31ReportBL.Load(v.Rec.x31ID_Payment).x31Name;
                }                                
                if (v.Rec.x38ID > 0)
                {
                    v.ComboX38Name = Factory.x38CodeLogicBL.Load(v.Rec.x38ID).x38Name;
                }
                if (v.Rec.x38ID_Payment > 0)
                {
                    v.ComboX38Name = Factory.x38CodeLogicBL.Load(v.Rec.x38ID_Payment).x38Name;
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

        private void RefreshState(p89Record v)
        {


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p89Record v, string oper)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.p89ProformaType c = new BO.p89ProformaType();
                if (v.rec_pid > 0) c = Factory.p89InvoiceTypeBL.Load(v.rec_pid);
                c.p89Name = v.Rec.p89Name;
                c.x38ID = v.Rec.x38ID;
                c.x38ID_Payment = v.Rec.x38ID_Payment;
                c.p93ID = v.Rec.p93ID;
                c.x31ID = v.Rec.x31ID;
                c.x31ID_Payment = v.Rec.x31ID_Payment;
               
                c.p89DefaultText1 = v.Rec.p89DefaultText1;
                c.p89DefaultText2 = v.Rec.p89DefaultText2;
               
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.p89InvoiceTypeBL.Save(c);
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

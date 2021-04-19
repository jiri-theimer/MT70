using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers.Admin
{
    public class p92Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new p92Record() { rec_pid = pid, rec_entity = "p92" };
            v.Rec = new BO.p92InvoiceT();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p92InvoiceTypeBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.ComboJ17Name = v.Rec.j17Name;
                v.ComboJ27Code = v.Rec.j27Code;
                v.ComboJ61Name = v.Rec.j61Name;
                v.ComboP93Name = v.Rec.p93Name;
                if (v.Rec.p80ID > 0)
                {
                    v.ComboP80Name = Factory.p80InvoiceAmountStructureBL.Load(v.Rec.p80ID).p80Name;
                }
                if (v.Rec.p98ID > 0)
                {
                    v.ComboP98Name = Factory.p98Invoice_Round_Setting_TemplateBL.Load(v.Rec.p98ID).p98Name;
                }
                if (v.Rec.x31ID_Invoice > 0)
                {
                    
                }
                if (v.Rec.x38ID > 0)
                {
                    v.ComboX38Name = Factory.x38CodeLogicBL.Load(v.Rec.x38ID).x38Name;
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

        private void RefreshState(p92Record v)
        {


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p92Record v,string oper)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.p92InvoiceT c = new BO.p92InvoiceT();
                if (v.rec_pid > 0) c = Factory.p92InvoiceTypeBL.Load(v.rec_pid);
                c.p92Name = v.Rec.p92Name;
                c.p92Ordinary = v.Rec.p92Ordinary;
                c.p92InvoiceType = v.Rec.p92InvoiceType;
                c.x38ID = v.Rec.x38ID;
                c.j27ID = v.Rec.j27ID;
                c.p93ID = v.Rec.p93ID;
                c.p80ID = v.Rec.p80ID;
                c.p98ID = v.Rec.p98ID;
                c.p92AccountingIDS = v.Rec.p92AccountingIDS;
                c.p92ClassificationVATIDS = v.Rec.p92ClassificationVATIDS;

                c.x31ID_Invoice = v.Rec.x31ID_Invoice;
                c.x31ID_Attachment = v.Rec.x31ID_Attachment;
                c.x31ID_Letter = v.Rec.x31ID_Letter;

                c.p92InvoiceDefaultText1 = v.Rec.p92InvoiceDefaultText1;
                c.p92InvoiceDefaultText2 = v.Rec.p92InvoiceDefaultText2;
                c.p92ReportConstantPreText1 = v.Rec.p92ReportConstantPreText1;
                c.p92ReportConstantText = v.Rec.p92ReportConstantText;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.p92InvoiceTypeBL.Save(c);
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

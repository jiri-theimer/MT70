using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class p93Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new p93Record() { rec_pid = pid, rec_entity = "p93", UploadGuidLogo=BO.BAS.GetGuid(),UploadGuidSignature=BO.BAS.GetGuid() };
            v.Rec = new BO.p93InvoiceHeader();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.p93InvoiceHeaderBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
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

        private void RefreshState(p93Record v)
        {


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p93Record v)
        {
            RefreshState(v);

            if (ModelState.IsValid)
            {
                BO.p93InvoiceHeader c = new BO.p93InvoiceHeader();
                if (v.rec_pid > 0) c = Factory.p93InvoiceHeaderBL.Load(v.rec_pid);
                c.p93Name = v.Rec.p93Name;
                c.p93Company = v.Rec.p93Company;
                c.p93City = v.Rec.p93City;
                c.p93Street = v.Rec.p93Street;
                c.p93Zip = v.Rec.p93Zip;
                c.p93RegID = v.Rec.p93RegID;
                c.p93VatID = v.Rec.p93VatID;
                c.p93Contact = v.Rec.p93Contact;
                c.p93Registration = v.Rec.p93Registration;
                c.p93Referent = v.Rec.p93Referent;
                c.p93Signature = v.Rec.p93Signature;
                c.p93FreeText01 = v.Rec.p93FreeText01;
                c.p93FreeText02 = v.Rec.p93FreeText02;
                c.p93FreeText03 = v.Rec.p93FreeText03;
                c.p93FreeText04 = v.Rec.p93FreeText04;

                c.p93LogoFile = v.LogoFile;
                c.p93SignatureFile = v.SignatureFile;

                c.p93Country = v.Rec.p93Country;
                c.p93CountryCode = v.Rec.p93CountryCode;
                c.p93Email = v.Rec.p93Email;
                c.p93ICDPH_SK = v.Rec.p93ICDPH_SK;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.p93InvoiceHeaderBL.Save(c,null);
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

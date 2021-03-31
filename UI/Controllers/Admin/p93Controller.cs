using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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

                if (v.Rec.p93LogoFile != null)
                {
                    v.LogoFile = "/Plugins/" + v.Rec.p93LogoFile;
                }
                if (v.Rec.p93SignatureFile != null)
                {
                    v.SignatureFile = "/Plugins/" + v.Rec.p93SignatureFile;
                }
                var lis = Factory.p93InvoiceHeaderBL.GetList_p88(v.rec_pid).ToList();
                v.lisP88 = new List<p88Repeater>();
                foreach(var c in lis)
                {
                    var recP86 = Factory.p86BankAccountBL.Load(c.p86ID);
                    v.lisP88.Add(new p88Repeater() {
                        TempGuid=BO.BAS.GetGuid(),j27ID=c.j27ID,p86ID=c.p86ID
                        ,ComboJ27=Factory.FBL.LoadCurrencyByID(c.j27ID).j27Code
                        ,ComboP86=recP86.p86BankAccount+"/"+recP86.p86BankCode+" ("+recP86.p86Name+")"
                    });
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
            if (v.lisP88 == null)
            {
                v.lisP88 = new List<p88Repeater>();
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(p93Record v,string oper,string guid)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "add_row")
            {
                var c = new p88Repeater() { TempGuid = BO.BAS.GetGuid() };                
                v.lisP88.Add(c);
                return View(v);
            }
            if (oper == "delete_row")
            {
                v.lisP88.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
           
                    
            if (oper == "delete_logo")
            {
                v.UploadGuidLogo = BO.BAS.GetGuid();
                v.IsDeleteLogo = true;
                this.AddMessage("Změny se potvrdí až po uložení záznamu.", "info");
                return View(v);
            }
            if (oper== "delete_signature")
            {
                v.UploadGuidSignature = BO.BAS.GetGuid();
                v.IsDeleteSignature = true;
                this.AddMessage("Změny se potvrdí až po uložení záznamu.", "info");
                return View(v);
            }
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
                
                c.p93Country = v.Rec.p93Country;
                c.p93CountryCode = v.Rec.p93CountryCode;
                c.p93Email = v.Rec.p93Email;
                c.p93ICDPH_SK = v.Rec.p93ICDPH_SK;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                if (v.IsDeleteLogo) c.p93LogoFile = null;
                if (v.IsDeleteSignature) c.p93SignatureFile = null;

                var lis = new List<BO.p88InvoiceHeader_BankAccount>();
                foreach (var row in v.lisP88.Where(p => p.IsTempDeleted == false))
                {
                    var cc = new BO.p88InvoiceHeader_BankAccount() { j27ID = row.j27ID,p86ID=row.p86ID };
                    lis.Add(cc);
                }

                c.pid = Factory.p93InvoiceHeaderBL.Save(c,lis);
                if (c.pid > 0)
                {
                    if (Factory.o27AttachmentBL.GetTempFiles(v.UploadGuidLogo).Count() > 0)
                    {
                        var tempfile = Factory.o27AttachmentBL.GetTempFiles(v.UploadGuidLogo).First();
                        var strOrigFileName = "p93_logo_" + c.pid.ToString() + "_original"+ tempfile.o27FileExtension;
                        System.IO.File.Copy(tempfile.FullPath, Factory.App.WwwRootFolder + "\\Plugins\\" + strOrigFileName, true);

                        var strDestFileName = "p93_logo_" + c.pid.ToString() + tempfile.o27FileExtension;
                        if (Factory.App.IsCloud)
                        {
                            strDestFileName = BO.BAS.ParseDbNameFromCloudLogin(Factory.CurrentUser.j03Login) + "_" + strDestFileName;
                        }
                        basUI.ResizeImage(tempfile.FullPath, Factory.App.WwwRootFolder + "\\Plugins\\" + strDestFileName, 250, 100);

                        c = Factory.p93InvoiceHeaderBL.Load(c.pid);
                        c.p93LogoFile = strDestFileName;
                        Factory.p93InvoiceHeaderBL.Save(c, null);
                    }
                    
                    if (Factory.o27AttachmentBL.GetTempFiles(v.UploadGuidSignature).Count() > 0)
                    {
                        var tempfile = Factory.o27AttachmentBL.GetTempFiles(v.UploadGuidSignature).First();
                        var strOrigFileName = "p93_signature_" + c.pid.ToString() + "_original"+tempfile.o27FileExtension;
                        System.IO.File.Copy(tempfile.FullPath, Factory.App.WwwRootFolder + "\\Plugins\\" + strOrigFileName, true);

                        var strDestFileName = "p93_signature_" + c.pid.ToString() + tempfile.o27FileExtension;
                        if (Factory.App.IsCloud)
                        {
                            strDestFileName = BO.BAS.ParseDbNameFromCloudLogin(Factory.CurrentUser.j03Login) + "_" + strDestFileName;
                        }
                        basUI.ResizeImage(tempfile.FullPath, Factory.App.WwwRootFolder + "\\Plugins\\" + strDestFileName, 300, 130);

                        c = Factory.p93InvoiceHeaderBL.Load(c.pid);
                        c.p93SignatureFile = strDestFileName;
                        Factory.p93InvoiceHeaderBL.Save(c, null);
                    }
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }


                    
                

            }


            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}

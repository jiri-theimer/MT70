﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using MimeKit;
using UI.Models.Record;

namespace UI.Controllers
{
    public class MailController : BaseController
    {
        private readonly BL.ThePeriodProvider _pp;
        public MailController(BL.ThePeriodProvider pp)
        {
            _pp = pp;
        }

        public IActionResult SendMail(int x40id,string uploadguid,int j02id,int x29id,int recpid,int x31id)
        {
            if (recpid == 0 || x29id == 0)
            {
                recpid = Factory.CurrentUser.j02ID;
                x29id = 102;
            }
            if (string.IsNullOrEmpty(uploadguid))
            {
                uploadguid = BO.BAS.GetGuid();
            }
            
            var v = new Models.SendMailViewModel() { UploadGuid = uploadguid };
            v.Rec = new BO.x40MailQueue();
           
            v.Rec.x29ID = (BO.x29IdEnum) x29id;
            v.Rec.x40RecordPID = recpid;
            if (j02id > 0)
            {
                v.Recipient = Factory.j02PersonBL.Load(j02id).j02Email;
            }


            v.Rec.o40ID = Factory.CBL.LoadUserParamInt("SendMail_o40ID"); 
            if (v.Rec.o40ID > 0)
            {
                v.SelectedO40Name = Factory.o40SmtpAccountBL.Load(v.Rec.o40ID).o40Name;
            }
            v.Rec.x40MessageID = BO.BAS.GetGuid();

            if (x31id > 0)  //tisková sestava bez kontextu odeslana do pošty
            {
                var recX31 = Factory.x31ReportBL.Load(x31id);
                var cc = new TheReportSupport();
                cc.GeneratePdfReport(Factory, _pp, recX31, v.UploadGuid,0);
            }

            if (v.Rec.x29ID == BO.x29IdEnum.p91Invoice && v.Rec.x40RecordPID>0)
            {
                //najít poštovní šablonu faktury klienta
                var recP91 = Factory.p91InvoiceBL.Load(v.Rec.x40RecordPID);
                var recP28 = Factory.p28ContactBL.Load(recP91.p28ID);
                if (recP28.j61ID_Invoice > 0)   //klient má svojí výchozí poštovní šablonu
                {
                    var recJ61 = MailMergeByTextTemplate(recP28.j61ID_Invoice, "p91", v.Rec.x40RecordPID);
                    v.Rec.x40Body = recJ61.j61PlainTextBody;
                    v.Rec.x40Subject = recJ61.j61MailSubject;
                    v.SelectedJ61ID = recJ61.pid;
                    v.SelectedJ61Name = recJ61.j61Name;
                }
            }


            if (x40id > 0)
            {   //kopírování zprávy do nové podle vzoru x40id
                v.Rec = Factory.MailBL.LoadMessageByPid(x40id);
                v.Recipient = v.Recipient;
                v.Rec.x40CC = v.Rec.x40CC;
                v.Rec.x40BCC = v.Rec.x40BCC;
                v.Rec.x40Subject = v.Rec.x40Subject;
                v.Rec.x40Body = v.Rec.x40Body;

                var vtemp = new x40RecMessage();
                vtemp.Rec = v.Rec;
                InhaleMimeMessage(ref vtemp, v.UploadGuid);
                v.Rec.x40MessageID = BO.BAS.GetGuid();    //jednoznačný guid do nové zprávy

            }

            return View(v);
        }
        [HttpPost]
        public IActionResult SendMail(Models.SendMailViewModel v,string oper)
        {                        
            if (oper == "j11id" && v.SelectedJ11ID>0)
            {               
                Handle_Receiver_From_List(v, Factory.j02PersonBL.GetList(new BO.myQueryJ02() { j11id = v.SelectedJ11ID }));
                return View(v);
            }
            if (oper == "j07id" && v.SelectedJ07ID > 0)
            {
                Handle_Receiver_From_List(v, Factory.j02PersonBL.GetList(new BO.myQueryJ02() { j07id = v.SelectedJ07ID }));
                return View(v);
            }
            

            if (!string.IsNullOrEmpty(oper))
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                foreach (BO.o27Attachment c in Factory.o27AttachmentBL.GetTempFiles(v.UploadGuid))
                {
                    Factory.MailBL.AddAttachment(c.FullPath, c.o27OriginalFileName, c.o27ContentType);
                }

                
                v.Rec.x40Recipient = v.Recipient;
                var ret = Factory.MailBL.SendMessage(v.Rec, v.IsTest);



                if (v.Rec.o40ID > 0)
                {
                    Factory.CBL.SetUserParam("SendMail_o40ID", v.Rec.o40ID.ToString());                    
                }

                if (ret.Flag == BO.ResultEnum.Success)  //případná chybová hláška je již naplněná v BL vrstvě
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }


            }

            return View(v);
        }

        private void Handle_Receiver_From_List(Models.SendMailViewModel v,IEnumerable<BO.j02Person> lisJ02)
        {
            if (!string.IsNullOrEmpty(v.Recipient))
            {
                v.Recipient = v.Recipient.Replace(";", ",");
            }
            
            var lis = BO.BAS.ConvertString2List(v.Recipient,",");
            foreach(var c in lisJ02.Where(p=>p.j02Email != null).OrderBy(p=>p.FullNameDesc))
            {                
                if (lis.Where(p=>p.ToLower()==c.j02Email.ToLower()).Count()==0)
                {
                    lis.Add(c.j02Email);
                }
            }
            v.Recipient = string.Join(", ", lis.Where(p=>p.Trim().Length>2));

        }

        public IEnumerable<MailItemAutoComplete> AutoCompleteSource()
        {
            var ret = new List<MailItemAutoComplete>();

            var lis0 = Factory.MailBL.GetAllx43Emails();
            foreach (string email in lis0)
            {
                ret.Add(new MailItemAutoComplete() { address = email.ToLower() });
            }

            var mqJ02 = new BO.myQueryJ02();
            new BO.myQueryJ02().explicit_orderby = "a.j02LastName";
            var lis1 = Factory.j02PersonBL.GetList(mqJ02).Where(p=>p.j02Email !=null);
            foreach(var c in lis1)
            {
                if (!ret.Any(p=>p.address==c.j02Email.ToLower()))
                {
                    ret.Insert(0,new MailItemAutoComplete() { address = c.j02Email, text = c.FullNameDesc });
                }
                
            }

            
            return ret;
        }
    
        public IActionResult Record(int pid)
        {
            var v = new Models.x40RecMessage();
            v.Rec = Factory.MailBL.LoadMessageByPid(pid);


            if (v.Rec == null)
            {
                return RecNotFound(v);
            }


            InhaleMimeMessage(ref v, v.Rec.x40MessageID);

            return View(v);
        }



        public ActionResult DownloadEmlFile(string guid)
        {
            var rec = Factory.MailBL.LoadMessageByGuid(guid);
            if (rec == null)
            {
                return this.NotFound(new x40RecMessage());

            }
            string fullPath = Factory.x35GlobalParamBL.UploadFolder() + "\\" + rec.x40ArchiveFolder + "\\" + rec.x40MessageID + ".eml";


            if (System.IO.File.Exists(fullPath))
            {
                Response.Headers["Content-Length"] = rec.x40EmlFileSize.ToString();
                Response.Headers["Content-Disposition"] = "inline; filename=poštovní_zpráva.eml";
                var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(fullPath), "message/rfc822");

                return fileContentResult;
                //return File(System.IO.File.ReadAllBytes(fullPath), "message/rfc822", "poštovní_zpráva.eml");
            }
            else
            {
                return RedirectToAction("FileDownloadNotFound", "o23");
            }



        }


        private void InhaleMimeMessage(ref x40RecMessage v, string strDestGUID)
        {
            string fullPath = Factory.x35GlobalParamBL.UploadFolder() + "\\" + v.Rec.x40ArchiveFolder + "\\" + v.Rec.x40MessageID + ".eml";

            if (System.IO.File.Exists(fullPath) == false)
            {
                return;
            }

            v.MimeMessage = MimeMessage.Load(fullPath);
            v.MimeAttachments = new List<BO.StringPair>();

            foreach (var attachment in v.MimeMessage.Attachments)
            {
                if (attachment is MessagePart)
                {

                }
                else
                {
                    var part = (MimePart)attachment;
                    var fileName = part.FileName;
                    v.MimeAttachments.Add(new BO.StringPair() { Key = part.ContentType.MimeType, Value = fileName });

                    string strTempFullPath = this.Factory.x35GlobalParamBL.TempFolder() + "\\" + strDestGUID + "_" + fileName;
                    if (System.IO.File.Exists(strTempFullPath) == false)
                    {
                        string strInfoxFullPath = Factory.x35GlobalParamBL.TempFolder() + "\\" + strDestGUID + "_" + fileName + ".infox";
                        System.IO.File.WriteAllText(strInfoxFullPath, part.ContentType.MimeType + "|0| " + fileName + "|" + strDestGUID + "_" + fileName + "|" + strDestGUID + "|0||");



                        using (var fs = new FileStream(strTempFullPath, System.IO.FileMode.Create))
                        {
                            part.Content.DecodeTo(fs);  //uložit attachment soubor do tempu



                        }
                    }

                }


            }


        }

        public BO.j61TextTemplate MailMergeByTextTemplate(int j61id,string prefix,int recpid)
        {
            var recJ61 = Factory.j61TextTemplateBL.Load(j61id);
            var cMerge = new BO.CLS.MergeContent();
            var dt = Factory.gridBL.GetList4MailMerge(prefix, recpid);
            recJ61.j61PlainTextBody = cMerge.GetMergedContent(recJ61.j61PlainTextBody, dt);
            recJ61.j61MailSubject = cMerge.GetMergedContent(recJ61.j61MailSubject, dt);

            return recJ61;
        }


       
    }



}
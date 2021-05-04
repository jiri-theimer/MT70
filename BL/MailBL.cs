using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Linq;
using BO;
using System.IO;

namespace BL
{
    public interface IMailBL
    {
        public BO.Result SendMessage(int j40id, MailMessage message); //v Result.pid vrací x40id
        public BO.Result SendMessage(int j40id, string toEmail, string toName, string subject, string body, bool ishtml,int x29id,int recpid); //v Result.pid vrací x40id
        public BO.Result SendMessage(BO.x40MailQueue rec,bool istest);  //v Result.pid vrací x40id
        public void AddAttachment(string fullpath, string displayname, string contenttype = null);
        public void AddAttachment(Attachment att);
       
        public BO.x40MailQueue LoadMessageByPid(int x40id);
        public BO.x40MailQueue LoadMessageByGuid(string guid);
        public bool SaveMailJob2Temp(string strJobGuid, BO.x40MailQueue recX40, string strUploadGuid, List<BO.x43MailQueue_Recipient> lisX43);
        public IEnumerable<BO.x40MailQueue> GetList(BO.myQueryX40 mq);
        public int SaveX40(MailMessage m, BO.x40MailQueue rec);
        public void StopPendingMessagesInBatch(string batchguid);
        public void RestartMessagesInBatch(string batchguid);
        public List<string> GetAllx43Emails();



    }
    class MailBL : BaseBL, IMailBL
    {
        private BO.o40SmtpAccount _account;
        public MailBL(BL.Factory mother) : base(mother)
        {

        }
        private List<Attachment> _attachments;        

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,case when a.x40DatetimeProcessed is not null then a.x40DatetimeProcessed else a.x40DateInsert end as MessageTime,");
            sb(_db.GetSQL1_Ocas("x40", false, false));
            sb(" FROM x40MailQueue a");
            sb(strAppend);
            return sbret();
        }
        public BO.x40MailQueue LoadMessageByPid(int x40id)
        {
            sb(GetSQL1(" WHERE a.x40ID=@pid"));               
            return _db.Load<BO.x40MailQueue>(sbret(), new { pid = x40id });
        }
        public BO.x40MailQueue LoadMessageByGuid(string guid)
        {
            sb(GetSQL1(" WHERE a.x40MessageID=@guid"));            
            return _db.Load<BO.x40MailQueue>(sbret(), new { guid = guid });
        }

        public void AddAttachment(string fullpath,string displayname,string contenttype=null)
        {
            if (_attachments == null) _attachments = new List<Attachment>();
            var att = new Attachment(fullpath);
            att.Name = displayname;
            if (contenttype != null)
            {
                att.ContentType = new System.Net.Mime.ContentType(contenttype);
            }            
            _attachments.Add(att);
        }
        public void AddAttachment(Attachment att)
        {
            if (_attachments == null) _attachments = new List<Attachment>();
            _attachments.Add(att);
        }
        private BO.x40MailQueue InhaleMessageSender(int o40id,BO.x40MailQueue rec)
        {
            if (o40id > 0)
            {
                _account = _mother.o40SmtpAccountBL.Load(o40id);
            }
            else
            {
                _account = _mother.o40SmtpAccountBL.LoadGlobalDefault();        //výchozí (globální) SMTP účet         
            }
            if (_account == null)
            {
                return new BO.x40MailQueue() { o40ID = 0 };
            }
            rec.o40ID = _account.pid;
            rec.x40SenderAddress = _account.o40EmailAddress;
            rec.x40SenderName = _account.o40Name;   
            
            if (_account.o40IsUsePersonalReply)
            {                
                rec.x40SenderName = _mother.CurrentUser.PersonAsc;
            }
                                    
            return rec;
        }
        public BO.Result SendMessage(int j40id, string toEmail, string toName, string subject, string body, bool ishtml, int x29id, int recpid)  //v BO.Result.pid vrací x40id
        {

            BO.x40MailQueue rec = new BO.x40MailQueue() { x40Recipient = toEmail, x40Subject = subject, x40Body = body, x40IsHtmlBody = ishtml,x40MessageID=BO.BAS.GetGuid(),x40RecordPID=recpid };
            rec.x29ID = (BO.x29IdEnum)x29id;
            rec = InhaleMessageSender(j40id,rec);
            return SendMessage(rec,false);
           
        }
        public BO.Result SendMessage(BO.x40MailQueue rec, bool istest)  //v BO.Result.pid vrací x40id
        {
            rec = InhaleMessageSender(rec.o40ID, rec);            
            MailMessage m = new MailMessage() { Body = rec.x40Body, Subject = rec.x40Subject,IsBodyHtml=rec.x40IsHtmlBody};                        

            m.From = new MailAddress(rec.x40SenderAddress, rec.x40SenderName);
            var lis = new List<string>();
            if (String.IsNullOrEmpty(rec.x40Recipient) == false)
            {
                lis = BO.BAS.ConvertString2List(rec.x40Recipient.Replace(";", ","), ",");
                foreach (string s in lis.Where(p=>string.IsNullOrEmpty(p.Trim())==false))
                {
                    m.To.Add(new MailAddress(s.Trim()));
                }
            }
            if (String.IsNullOrEmpty(rec.x40CC) == false)
            {
                lis = BO.BAS.ConvertString2List(rec.x40CC.Replace(";", ","), ",");
                foreach (string s in lis)
                {
                    m.CC.Add(new MailAddress(s));
                }
            }
            if (String.IsNullOrEmpty(rec.x40BCC) == false)
            {
                lis = BO.BAS.ConvertString2List(rec.x40BCC.Replace(";", ","), ",");
                foreach (string s in lis)
                {
                    m.Bcc.Add(new MailAddress(s));
                }
            }

           

            return handle_smtp_finish(m, rec,istest);
        }
        public BO.Result SendMessage(int o40id, MailMessage message)
        {
            var rec = new BO.x40MailQueue();
            if (message.From == null)
            {
                rec = InhaleMessageSender(o40id, rec);
                message.From = new MailAddress(rec.x40SenderAddress, rec.x40SenderName);
            }
            return handle_smtp_finish(message,rec,false);
        }


        private BO.Result handle_smtp_finish(MailMessage m,BO.x40MailQueue rec, bool istest)     //finální odeslání zprávy
        {
            if (rec.x40MessageID == null)
            {
                rec.x40MessageID = BO.BAS.GetGuid();
            }
            if (_account == null)
            {
                return handle_result_error("Chybí poštovní účet odesílatele");
            }
            if (m.From == null)
            {
                return handle_result_error( "Chybí odesílatel zprávy");
            }
            if (m.To.Count == 0)
            {
                return handle_result_error("Chybí příjemce zprávy");
            }
            if (string.IsNullOrEmpty(m.Body) && string.IsNullOrEmpty(m.Subject))
            {
                return handle_result_error("Chybí předmět nebo text zprávy.");
            }
            
           
            if (_account.o40IsUsePersonalReply)
            {
                var recJ02 = _mother.j02PersonBL.Load(_mother.CurrentUser.j02ID);
                m.ReplyToList.Add(new MailAddress(recJ02.j02Email, recJ02.FullNameAsc));
            }

            BO.Result ret = new BO.Result(false);
            
            using (SmtpClient client = new SmtpClient(_account.o40Server, _account.o40Port))
            {
                if (_account.o40IsVerify)
                {
                    client.Credentials = new System.Net.NetworkCredential(_account.o40Login, _account.o40Password);                    
                }
                else
                {
                    client.UseDefaultCredentials = true;
                }
                
                
                m.BodyEncoding = Encoding.UTF8;
                m.SubjectEncoding = Encoding.UTF8;
                m.Headers.Add("Message-ID", rec.x40MessageID);


                if (_attachments != null)
                {
                    foreach (var att in _attachments)
                    {
                        m.Attachments.Add(att);
                    }
                }
                              
                
               
                if (1 == 1) //postup, jak odeslanou zprávu uložit na serveru jako EML soubor:
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    
                    client.PickupDirectoryLocation = _mother.x35GlobalParamBL.TempFolder();
                    client.Send(m);//nejdříve uložit eml soubor do temp složky
                    string strFullPath = FindEmlFileByGuid(rec.x40MessageID); //najít vygenerovaný eml file podle jeho Message-ID
                    if (strFullPath != "")
                    {
                        rec.x40ArchiveFolder = "eml\\" + DateTime.Now.Year.ToString() + "\\" + DateTime.Now.Month.ToString();
                        rec.x40EmlFileSize = (int)(new System.IO.FileInfo(strFullPath).Length);
                        if (!System.IO.Directory.Exists(_mother.x35GlobalParamBL.UploadFolder() + "\\" + rec.x40ArchiveFolder))
                        {
                            System.IO.Directory.CreateDirectory(_mother.x35GlobalParamBL.UploadFolder() + "\\" + rec.x40ArchiveFolder);
                        }
                        string strDestPath = _mother.x35GlobalParamBL.UploadFolder() + "\\" + rec.x40ArchiveFolder + "\\" + rec.x40MessageID + ".eml";
                        if (!File.Exists(strDestPath))
                        {
                            File.Move(strFullPath, strDestPath);    //přejmenovat nalezený eml file na guid
                        }


                    }
                }
                
                client.DeliveryMethod = SmtpDeliveryMethod.Network; //nyní opravdu odeslat
                if (_account.o40SslModeFlag==BO.SslModeENUM.Explicit || _account.o40SslModeFlag == BO.SslModeENUM.Implicit)
                {
                    client.EnableSsl = true;
                }
                
                try
                {
                    if (istest == false)
                    {
                        client.Send(m);                        
                    }                    
                    if (istest)
                    {
                        rec.x40ErrorMessage = "Testovací režim";
                    }
                    else
                    {
                        rec.x40ErrorMessage = "";
                    }                    
                    rec.x40State = BO.x40StateENUM.IsProceeded;
                    
                    ret.pid = SaveX40(m, rec);
                    ret.Flag = ResultEnum.Success;

                }
                catch (Exception ex)
                {

                    this.AddMessageTranslated(ex.Message);
                    rec.x40ErrorMessage = ex.Message;
                    rec.x40State = BO.x40StateENUM.IsError;
                    ret.pid = SaveX40(m, rec);
                    ret.Flag = ResultEnum.Failed;
                    ret.Message = rec.x40ErrorMessage;
                }

                
            }

            return ret;


        }

        public void StopPendingMessagesInBatch(string batchguid)
        {
            _db.RunSql("UPDATE x40MailQueue set x40Status=4 WHERE x40BatchGuid=@guid AND x40Status=1", new {guid = batchguid });
            
        }
        public void RestartMessagesInBatch(string batchguid)
        {
            _db.RunSql("UPDATE x40MailQueue set x40Status=1 WHERE x40BatchGuid=@guid AND x40Status=4", new { guid = batchguid });
            
        }

        public int SaveX40(MailMessage m,BO.x40MailQueue rec)
        {            
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            if (string.IsNullOrEmpty(rec.x40MessageID) == true)
            {
                rec.x40MessageID = BO.BAS.GetGuid();
            }
            p.AddString("x40MessageID", rec.x40MessageID);
            p.AddString("x40BatchGuid", rec.x40BatchGuid);
            p.AddInt("o40ID", rec.o40ID, true);
            p.AddEnumInt("x29ID", rec.x29ID, true);
            if (rec.j03ID_Sys == 0) rec.j03ID_Sys = _mother.CurrentUser.pid;
            p.AddInt("j03ID_Sys", rec.j03ID_Sys, true);
            p.AddInt("x40RecordPID", rec.x40RecordPID, true);
            
            if (m != null)
            {
                p.AddString("x40SenderAddress", m.From.Address);
                p.AddString("x40SenderName", m.From.DisplayName);
                p.AddString("x40Recipient", String.Join(", ", m.To.Select(p => p.Address)));
                p.AddString("x40BCC", String.Join(",", m.Bcc.Select(p => p.Address)));
                p.AddString("x40CC", String.Join(",", m.CC.Select(p => p.Address)));
                p.AddString("x40Subject", m.Subject);
                p.AddString("x40Body", m.Body);
                p.AddBool("x40IsHtmlBody", m.IsBodyHtml);
                p.AddString("x40Attachments", String.Join(",", m.Attachments.Select(p => p.Name)));
            }
            else
            {
                p.AddString("x40SenderAddress", rec.x40SenderAddress);
                p.AddString("x40SenderName", rec.x40SenderName);
                p.AddString("x40Recipient", rec.x40Recipient);
                p.AddString("x40BCC", rec.x40BCC);
                p.AddString("x40CC", rec.x40CC);
                p.AddString("x40Subject", rec.x40Subject);
                p.AddString("x40Body", rec.x40Body);
                p.AddBool("x40IsHtmlBody", rec.x40IsHtmlBody);
                p.AddString("x40Attachments", rec.x40Attachments);
            }
                        
            p.AddDateTime("x40WhenProceeded", rec.x40WhenProceeded);
            p.AddString("x40ErrorMessage", rec.x40ErrorMessage);
            p.AddEnumInt("x40State", rec.x40State);
            p.AddBool("x40IsAutoNotification", rec.x40IsAutoNotification);
            
            p.AddString("x40ArchiveFolder", rec.x40ArchiveFolder);
            
            
            return _db.SaveRecord("x40MailQueue", p, rec,false,true);
        }

        private BO.Result handle_result_error(string strError)
        {
            this.AddMessageTranslated(strError);
            return new BO.Result(true, strError);
        }

        private string FindEmlFileByGuid(string strGUID)
        {

            DirectoryInfo dir = new DirectoryInfo(_mother.x35GlobalParamBL.TempFolder());
            
            foreach (FileInfo file in dir.GetFiles("*.eml").OrderByDescending(p => p.CreationTime))
            {
                StreamReader reader = file.OpenText();
                string s = "";
                while ((s = reader.ReadLine()) !=null)
                {
                    if (s.Contains("Message-ID"))
                    {
                        if (s.Contains(strGUID))
                        {
                            reader.Close();
                            return file.FullName;                           
                        }
                        reader.Close();
                        break;
                    }                    
                }

            }

            return "";
        }

        public bool SaveMailJob2Temp(string strJobGuid,BO.x40MailQueue recX40,string strUploadGuid, List<BO.x43MailQueue_Recipient> lisX43)
        {
            var recTemp = new BO.p85Tempbox() { p85Prefix = "x40", p85GUID = strJobGuid, p85FreeText01 = recX40.x40Subject,p85Message=recX40.x40Body,p85FreeText04=strUploadGuid };
            if (_mother.p85TempboxBL.Save(recTemp) == 0)
            {
                return false;
            }
            foreach (var c in lisX43)
            {
                recTemp = new BO.p85Tempbox() { p85Prefix = "x43", p85GUID = strJobGuid, p85FreeText01 = c.x43Email, p85FreeText02 = c.x43DisplayName, p85OtherKey3=(int)c.x43RecipientFlag };
                _mother.p85TempboxBL.Save(recTemp);
              
            }

            return true;
        }

        public IEnumerable<BO.x40MailQueue> GetList(BO.myQueryX40 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql("SELECT a.*,"+ _db.GetSQL1_Ocas("x40",false,false,true)+" FROM x40MailQueue a", mq, _mother.CurrentUser);
            return _db.GetList<BO.x40MailQueue>(fq.FinalSql, fq.Parameters);
        }


        public List<string> GetAllx43Emails()
        {
            
            return _db.GetList<BO.GetString>("select distinct x43Email as Value FROM x43MailQueue_Recipient").Select(p => p.Value).ToList();

          
        }

    }
}

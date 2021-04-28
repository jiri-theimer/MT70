﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij03UserBL
    {
        public BO.j03User Load(int pid);
        public BO.j03User LoadByLogin(string strLogin, int pid_exclude);
        public BO.j03User LoadByJ02(int j02id);
        public IEnumerable<BO.j03User> GetList(BO.myQuery mq);
        public int Save(BO.j03User rec);
        public int SaveWithNewPersonalProfile(BO.j03User rec, BO.j02Person recJ02);
        public void UpdateCurrentUserPing(BO.j92PingLog c);
        public void RecoveryUserCache(int j03id,int j02id);
        public void TruncateUserParams(int j03id);

    }
    class j03UserBL : BaseBL, Ij03UserBL
    {
        public j03UserBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j03"));
            sb(",j04.j04Name,j02.j02Email");
            sb(",j02.j02TitleBeforeName as _j02TitleBeforeName,j02.j02FirstName as _j02FirstName,j02.j02LastName as _j02LastName");
            sb(" FROM j03User a");
            sb(" INNER JOIN j04UserRole j04 ON a.j04ID = j04.j04ID LEFT OUTER JOIN j02Person j02 ON a.j02id=j02.j02id");            
            sb(strAppend);
            return sbret();

            
        }
        public BO.j03User Load(int intPID)
        {
            return _db.Load<BO.j03User>(GetSQL1(" WHERE a.j03ID=@pid"), new { pid = intPID });
        }
       
        public BO.j03User LoadByLogin(string strLogin, int pid_exclude)
        {
            if (pid_exclude > 0)
            {
                return _db.Load<BO.j03User>(GetSQL1(" WHERE a.j03Login LIKE @login AND a.j03ID<>@pid_exclude"), new { login = strLogin, pid_exclude = pid_exclude });
            }
            else
            {
                return _db.Load<BO.j03User>(GetSQL1(" WHERE a.j03Login LIKE @login"), new { login = strLogin });
            }            
        }
        public BO.j03User LoadByJ02(int j02id)
        {
            return _db.Load<BO.j03User>(GetSQL1(" WHERE a.j02ID=@pid"), new { pid = j02id });
        }
        public IEnumerable<BO.j03User> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j03User>(fq.FinalSql, fq.Parameters);
        }
        
        public int SaveWithNewPersonalProfile(BO.j03User rec,BO.j02Person recJ02)
        {            
            if (_mother.j02PersonBL.ValidateBeforeSave(recJ02)==false)
            {
                return 0;
            }
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            rec.j02ID = _mother.j02PersonBL.Save(recJ02,null,null);
            if (rec.j02ID == 0)
            {
                this.AddMessage("Nepodařilo se založit osobní profil uživatele."); return 0;
            }

            return Save(rec);
        }

        public int Save(BO.j03User rec)
        {            
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddInt("j02ID", rec.j02ID,true);
            p.AddInt("j04ID", rec.j04ID,true);
            p.AddString("j03Login", rec.j03Login);
                       
            p.AddBool("j03IsSystemAccount", rec.j03IsSystemAccount);
            p.AddBool("j03IsMustChangePassword", rec.j03IsMustChangePassword);
            p.AddBool("j03IsShallReadUpgradeInfo", rec.j03IsShallReadUpgradeInfo);
            
            p.AddEnumInt("j03Ping_DeviceTypeFlag", rec.j03Ping_DeviceTypeFlag);
            p.AddInt("j03Ping_InnerWidth", rec.j03Ping_InnerWidth);
            p.AddInt("j03Ping_InnerHeight", rec.j03Ping_InnerHeight);
            p.AddInt("j03LangIndex", rec.j03LangIndex);
            
            p.AddBool("j03IsDebugLog", rec.j03IsDebugLog);
            p.AddInt("j03GridSelectionModeFlag", rec.j03GridSelectionModeFlag);
            p.AddDateTime("j03LiveChatTimestamp", rec.j03LiveChatTimestamp);
            p.AddString("j03HomePageUrl",rec.j03HomePageUrl);
            p.AddString("j03SiteMenuMyLinksV7", rec.j03SiteMenuMyLinksV7);
            p.AddString("j03PageSplitterFlagV7", rec.j03PageSplitterFlagV7);
            p.AddInt("j03HoursEntryFlagV7", rec.j03HoursEntryFlagV7);

            p.AddString("j03DefaultHoursFormat", rec.j03DefaultHoursFormat);
            p.AddInt("j03GlobalCssFlag", rec.j03GlobalCssFlag);
            p.AddInt("j03ModalWindowsFlag", rec.j03ModalWindowsFlag);
            
            if (!String.IsNullOrEmpty(rec.j03PasswordHash))
            {
                p.Add("j03PasswordHash", rec.j03PasswordHash);
            }


            var intPID = _db.SaveRecord("j03User", p, rec);
            if (intPID > 0)
            {
                RecoveryUserCache(intPID, 0);
            }
            return intPID;
        }

        private bool ValidateBeforeSave(BO.j03User rec)
        {
            if (rec.pid > 0 && rec.j02ID==0 && rec.j03IsSystemAccount == false)
            {
                this.AddMessage("U uživatelského účtu chybí vazba na osobní profil."); return false;
            }
            if (rec.pid > 0 && rec.j02ID == 0)
            {
                this.AddMessage("U uživatelského účtu chybí vazba na osobní profil."); return false;
            }
            if (string.IsNullOrEmpty(rec.j03Login))
            {
                this.AddMessage("Chybí vyplnit [Login]."); return false;
            }
            if (rec.j04ID==0)
            {
                this.AddMessage("Chybí vyplnit [Aplikační role]."); return false;
            }
            if (rec.j03Login.Contains(" "))
            {
                this.AddMessage("Přihlašovací jméno nesmí obsahovat znak [mezera]."); return false;
            }
            if (LoadByLogin(rec.j03Login, rec.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("V systému již existuje jiný uživatel s přihlašovacím jménem [{0}]."), rec.j03Login));
                return false;
            }
            if (rec.pid > 0)
            {
                var cDupl = LoadByJ02(rec.j02ID);
                if (cDupl != null && cDupl.pid != rec.pid)
                {
                    this.AddMessageTranslated(string.Format(_mother.tra("Osobní profil [{0}] již má zavedený jiný uživatelský účet [{1}]."), cDupl.PersonAsc,cDupl.j03Login));
                    return false;
                }
            }
            

            return true;
        }

        public void RecoveryUserCache(int j03id,int j02id)
        {
            if (j02id > 0)
            {
                j03id = _db.GetIntegerFromSql("select j03ID FROM j03User WHERE j02ID=" + j02id.ToString());
            }
            _db.RunSql("exec dbo.j03_recovery_cache @j03id,@j02id", new { j03id = j03id,j02id=j02id });
        }

        public void UpdateCurrentUserPing(BO.j92PingLog c) //zápis pravidelně po 2 minutách do PING logu
        {
            _db.RunSql("UPDATE j03User set j03Ping_Timestamp=GETDATE() WHERE j03ID=@pid", new { pid = _mother.CurrentUser.pid });    //ping aktualizace

            string s = "INSERT INTO j92PingLog(j03ID,j92Date,j92BrowserUserAgent,j92BrowserFamily,j92BrowserOS,j92BrowserDeviceType,j92BrowserDeviceFamily,j92BrowserAvailWidth,j92BrowserAvailHeight,j92BrowserInnerWidth,j92BrowserInnerHeight,j92RequestUrl)";
            s += " VALUES(@j03id,GETDATE(),@useragent,@browser,@os,@devicetype,@devicefamily,@aw,@ah,@iw,@ih,@requesturl)";
            _db.RunSql(s, new { j03id = _mother.CurrentUser.pid, useragent = c.j92BrowserUserAgent, browser = c.j92BrowserFamily, os = c.j92BrowserOS, devicetype = c.j92BrowserDeviceType, devicefamily = c.j92BrowserDeviceFamily, aw = c.j92BrowserAvailWidth, ah = c.j92BrowserAvailHeight, iw = c.j92BrowserInnerWidth, ih = c.j92BrowserInnerHeight, requesturl=c.j92RequestURL });


            if (_mother.CurrentUser.j03LiveChatTimestamp != null)   //hlídat, aby se automaticky vypnul live-chat box po 20ti minutách
            {
                if (_mother.CurrentUser.j03LiveChatTimestamp.Value.AddMinutes(20) < DateTime.Now)
                {                    
                    var rec = Load(_mother.CurrentUser.pid);
                    rec.j03LiveChatTimestamp = null;   //vypnout smartsupp
                    Save(rec);
                }
            }

        }

        public void TruncateUserParams(int j03id)
        {
            if (j03id <= 0) j03id = _mother.CurrentUser.pid;
            _db.RunSql("DELETE FROM x36UserParam WHERE j03ID=@j03id", new { j03id = j03id });
        }


    }
}

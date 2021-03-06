﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public interface Ij02PersonBL
    {
        public BO.j02Person Load(int pid);
        public BO.j02Person LoadByEmail(string strEmail, int pid_exclude);
        public BO.j02Person LoadByCode(string strCode, int pid_exclude);
        public IEnumerable<BO.j02Person> GetList(BO.myQueryJ02 mq);
        public IEnumerable<BO.j02Person> GetList_InP28Form(int p28id, string tempclientguid);
        public int Save(BO.j02Person rec, List<BO.FreeFieldInput> lisFFI,string tempguid);
        public bool ValidateBeforeSave(BO.j02Person rec);
        public BO.j02PersonSum LoadSumRow(int pid);
        public IEnumerable<BO.j02Person> GetList_Slaves(int j02id, bool bolDispCreateP31, BO.j05Disposition_p31ENUM? dispslave = null);
        public BO.j02RecDisposition InhaleRecDisposition(BO.j02Person rec);



    }
    class j02PersonBL : BaseBL,Ij02PersonBL
    {
        public j02PersonBL(BL.Factory mother):base(mother)
        {
           
        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j02"));
            sb(",j07.j07Name,c21.c21Name,c21.c21ScopeFlag,j18.j18Name");
            sb(",j03.j03ID,j03.j03Login");
            sb(" FROM j02Person a");
            sb(" LEFT OUTER JOIN j03User j03 ON a.j02ID = j03.j02ID LEFT OUTER JOIN j04UserRole j04 ON j03.j04ID = j04.j04ID");
            sb(" LEFT OUTER JOIN j07PersonPosition j07 ON a.j07ID=j07.j07ID LEFT OUTER JOIN c21FondCalendar c21 ON a.c21ID=c21.c21ID LEFT OUTER JOIN j18Region j18 ON a.j18ID=j18.j18ID");
            
            sb(strAppend);
            return sbret();
        }
       
        public BO.j02Person Load(int intPID)
        {
            return _db.Load<BO.j02Person>(GetSQL1(" WHERE a.j02ID=@pid"), new { pid = intPID });            
        }
        public BO.j02Person LoadByEmail(string strEmail, int pid_exclude)
        {
            return _db.Load<BO.j02Person>(GetSQL1(" WHERE a.j02Email LIKE @email AND a.j02ID<>@pid_exclude"), new { email = strEmail, pid_exclude = pid_exclude });
        }
        public BO.j02Person LoadByCode(string strCode, int pid_exclude)
        {
            return _db.Load<BO.j02Person>(GetSQL1(" WHERE a.j02Code LIKE @code AND a.j02ID<>@pid_exclude"), new { code = strCode, pid_exclude = pid_exclude });
        }

        public IEnumerable<BO.j02Person>GetList(BO.myQueryJ02 mq)
        {
            
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j02Person>(fq.FinalSql,fq.Parameters);
        }

        public IEnumerable<BO.j02Person> GetList_InP28Form(int p28id,string tempclientguid)
        {
            sb(GetSQL1());
            sb(" WHERE a.j02ID IN (select p85DataPID FROM p85TempBox WHERE p85GUID=@guid AND p85Prefix='p30' and p85IsDeleted=0)");
            sb(" OR (a.j02ID IN (select j02ID FROM p30Contact_Person WHERE p28ID=@p28id) AND a.j02ID NOT IN (select p85DataPID FROM p85TempBox WHERE p85IsDeleted=1 AND p85Prefix='p30' AND p85GUID=@guid))");
            return _db.GetList<BO.j02Person>(sbret(), new { p28id = p28id, guid = tempclientguid });
        }

        public IEnumerable<BO.j02Person> GetList_Slaves(int j02id, bool bolDispCreateP31,BO.j05Disposition_p31ENUM? dispslave=null)
        {
            sb(GetSQL1());
            sb(" WHERE a.j02ID IN (SELECT j02ID_Slave FROM j05MasterSlave WHERE j02ID_Master=@j02id_me");
            var arr = new List<string>();
            if (dispslave != null)
            {
                arr.Add("j05Disposition_p31 = " + ((int)dispslave).ToString());
            }
            if (bolDispCreateP31)
            {
                arr.Add("j05IsCreate_p31=1");
            }
            if (arr.Count> 0)
            {
                sb(" AND (");
                sb(string.Join(" OR ", arr));
                sb(")");
            }
            sb(")");
            sb(" OR a.j02ID IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_me");
            if (arr.Count > 0)
            {
                sb(" AND (");
                sb(string.Join(" OR ", arr));
                sb(")");
            }
            sb(")");
            sb(" ORDER BY a.j02LastName,a.j02FirstName");
            return _db.GetList<BO.j02Person>(sbret(), new { j02id_me = j02id });
        }

        public int Save(BO.j02Person rec, List<BO.FreeFieldInput> lisFFI,string tempguid)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            using (var sc = new System.Transactions.TransactionScope()) //ukládání podléhá transakci
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddInt("j07ID", rec.j07ID, true);
                p.AddInt("j17ID", rec.j17ID, true);
                p.AddInt("j18ID", rec.j18ID, true);
                p.AddInt("c21ID", rec.c21ID, true);
                p.AddEnumInt("p72ID_NonBillable", rec.p72ID_NonBillable);

                p.AddString("j02FirstName", rec.j02FirstName);
                p.AddString("j02LastName", rec.j02LastName);
                p.AddString("j02TitleBeforeName", rec.j02TitleBeforeName);
                p.AddString("j02TitleAfterName", rec.j02TitleAfterName);

                p.AddBool("j02IsIntraPerson", rec.j02IsIntraPerson);
                p.AddString("j02Email", rec.j02Email);
                p.AddString("j02EmailSignature", rec.j02EmailSignature);
                p.AddString("j02Code", rec.j02Code);

                p.AddString("j02Mobile", rec.j02Mobile);
                p.AddString("j02Phone", rec.j02Phone);
                p.AddString("j02JobTitle", rec.j02JobTitle);
                p.AddString("j02Office", rec.j02Office);
                p.AddString("j02ExternalPID", rec.j02ExternalPID);
                p.AddString("j02Description", rec.j02Description);

                p.AddInt("j02TimesheetEntryDaysBackLimit", rec.j02TimesheetEntryDaysBackLimit);
                p.AddString("j02TimesheetEntryDaysBackLimit_p34IDs", rec.j02TimesheetEntryDaysBackLimit_p34IDs);

                p.AddBool("j02IsInvoiceEmail", rec.j02IsInvoiceEmail);
                p.AddString("j02Salutation", rec.j02Salutation);
                p.AddString("j02InvoiceSignatureFile", rec.j02InvoiceSignatureFile);
                p.AddInt("j02WorksheetAccessFlag", rec.j02WorksheetAccessFlag);
                p.AddInt("j02NotifySubscriberFlag", rec.j02NotifySubscriberFlag);

                int intPID = _db.SaveRecord("j02Person", p, rec);
                if (intPID > 0)
                {
                    if (!string.IsNullOrEmpty(tempguid))
                    {
                        
                        _db.RunSql("INSERT INTO p85TempBox(p85GUID,p85Prefix,p85DataPID) VALUES(@guid,'j02',@pid)", new { guid = tempguid, pid = intPID });
                    }

                    _db.RunSql("exec dbo.j02_aftersave @j02id,@j03id_sys", new { j02id = intPID, j03id_sys = _mother.CurrentUser.pid });
                    
                    if (!DL.BAS.SaveFreeFields(_db, intPID, lisFFI))
                    {
                        return 0;
                    }

                    
                   

                    sc.Complete();
                    return intPID;
                }

                return 0;
                
            }
            
            
        }

        public bool ValidateBeforeSave(BO.j02Person rec)
        {
            if (string.IsNullOrEmpty(rec.j02FirstName))
            {
                this.AddMessage("Chybí vyplnit [Jméno]."); return false;
            }
            if (string.IsNullOrEmpty(rec.j02LastName))
            {
                this.AddMessage("Chybí vyplnit [Příjmení]."); return false;
            }
            if (string.IsNullOrEmpty(rec.j02Email))
            {
                this.AddMessage("Chybí vyplnit [E-mail]."); return false;
            }
            if (!BO.BAS.IsValidEmail(rec.j02Email))
            {
                this.AddMessage("Zadaná e-mail adresa není platná"); return false;
            }
            if (LoadByEmail(rec.j02Email,rec.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("E-mail adresa [{0}] již je obsazena jinou osobou."), rec.j02Email));
                return false;
            }
            if (rec.j02Code != null)
            {
                if (LoadByCode(rec.j02Code,rec.pid) != null)
                {
                    this.AddMessageTranslated(string.Format(_mother.tra("Osobní kód [{0}] již je obsazen jinou osobou."), rec.j02Code));
                    return false;
                }
            }

            return true;
        }

        public BO.j02PersonSum LoadSumRow(int pid)
        {
            return _db.Load<BO.j02PersonSum>("EXEC dbo.j02_inhale_sumrow @j03id_sys,@pid", new { j03id_sys = _mother.CurrentUser.pid, pid = pid });
        }

        public BO.j02RecDisposition InhaleRecDisposition(BO.j02Person rec)
        {
            var c = new BO.j02RecDisposition();

            if (_mother.CurrentUser.IsAdmin)
            {
                c.OwnerAccess = true; c.ReadAccess = true;
                return c;
            }
            if (rec.j02IsIntraPerson)
            {   //interní osoba
                var slaves = GetList_Slaves(rec.pid,false);
                if (slaves.Any(p=>p.pid==rec.pid))
                {
                    c.ReadAccess = true;                    
                }

            }
            else
            {
                //kontaktní osoba
                c.ReadAccess = true;                
            }
            

            return c;
        }
    }
}

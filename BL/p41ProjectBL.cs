using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip41ProjectBL
    {
        public BO.p41Project Load(int pid);
        public BO.p41Project LoadLastCreated(int levelindex);
        public BO.p41Project LoadByCode(string strCode, int pid_exclude);
        public BO.p41Project LoadByExternalPID(string externalpid);
        public IEnumerable<BO.p41Project> GetList(BO.myQueryP41 mq);
        public int Save(BO.p41Project rec, List<BO.FreeFieldInput> lisFFI, List<BO.x69EntityRole_Assign> lisX69);
        public BO.p41ProjectSum LoadSumRow(int pid);
        public BO.p41RecDisposition InhaleRecDisposition(BO.p41Project rec);
        public bool ExistWaitingWorksheetForInvoicing(int p41id);
        public bool ExistWaitingWorksheetForApproving(int p41id);

    }
    class p41ProjectBL : BaseBL, Ip41ProjectBL
    {
        public p41ProjectBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null,int intTopRecs=0)
        {
            sb("SELECT");
            if (intTopRecs > 0) sb($" TOP {intTopRecs}");
            sb(" a.p42ID,a.j02ID_Owner,a.p41Name,a.p41NameShort,a.p41Code,a.p41IsDraft,a.p28ID_Client,a.p28ID_Billing,a.p87ID,a.p51ID_Billing,a.p51ID_Internal,a.p92ID,a.b02ID,a.j18ID,a.p61ID,a.p41InvoiceDefaultText1,a.p41InvoiceDefaultText2,a.p41InvoiceMaturityDays,a.p41WorksheetOperFlag,a.p41PlanFrom,a.p41PlanUntil,a.p41LimitHours_Notification,a.p41LimitFee_Notification");
            sb(",p28client.p28Name as Client,p51billing.p51Name as p51Name_Billing,a.p41LimitWipFlag,p07.p07Level,p07.p07Name");
            sb(",a.p41TreeLevel,a.p41TreeIndex,a.p41TreePrev,a.p41TreeNext,a.p41TreePath,a.p41TreeOrdinary");
            sb(",p42.p42Name,p92.p92Name,b02.b02Name,j18.j18Name,a.p41ExternalPID,a.p41ParentID,a.p41BillingMemo");
            sb(",a.p41ID_P07Level1,a.p41ID_P07Level2,a.p41ID_P07Level3,a.p41ID_P07Level4");
            sb(",j02owner.j02LastName+' '+j02owner.j02FirstName as Owner,p28client.p87ID as p87ID_Client,p42.b01ID,a.p41IsNoNotify,a.p72ID_NonBillable,a.p72ID_BillableHours,a.j02ID_ContactPerson_DefaultInWorksheet,a.j02ID_ContactPerson_DefaultInInvoice");
            sb(",a.p41BillingFlag,a.p41ReportingFlag");
            sb(",p42.x38ID");
            sb(","+_db.GetSQL1_Ocas("p41"));

            sb(" FROM p41Project a INNER JOIN p42ProjectType p42 ON a.p42ID=p42.p42ID");
            sb(" LEFT OUTER JOIN p07ProjectLevel p07 ON p42.p07ID=p07.p07ID");
            sb(" LEFT OUTER JOIN p28Contact p28client ON a.p28ID_Client=p28client.p28ID");
            sb(" LEFT OUTER JOIN p51PriceList p51billing ON a.p51ID_Billing=p51billing.p51ID");
            sb(" LEFT OUTER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID");
            sb(" LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID");

            
            sb(" LEFT OUTER JOIN j18Region j18 ON a.j18ID=j18.j18ID");
            //sb(" LEFT OUTER JOIN p41Project_FreeField p41free ON a.p41ID=p41free.p41ID");
            sb(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID");

            sb(strAppend);


            return sbret();
        }
        public BO.p41Project Load(int pid)
        {
            return _db.Load<BO.p41Project>(GetSQL1(" WHERE a.p41ID=@pid"), new { pid = pid });
        }
        public BO.p41Project LoadLastCreated(int levelindex)
        {
            if (levelindex == 0)
            {
                return _db.Load<BO.p41Project>(GetSQL1(" WHERE a.p41UserInsert=@login ORDER BY a.p41ID DESC",1), new { login = _mother.CurrentUser.j03Login });
            }
            else
            {
                return _db.Load<BO.p41Project>(GetSQL1(" WHERE a.p41UserInsert=@login AND p07.p07Level=@level ORDER BY a.p41ID DESC",1), new { login = _mother.CurrentUser.j03Login,level=levelindex });
            }
            
        }
        public BO.p41Project LoadByCode(string strCode, int pid_exclude)
        {
            if (pid_exclude > 0)
            {
                return _db.Load<BO.p41Project>(GetSQL1(" WHERE a.p41Code LIKE @code AND a.p41ID<>@pid_exclude"), new { code = strCode, pid_exclude = pid_exclude });
            }
            else
            {
                return _db.Load<BO.p41Project>(GetSQL1(" WHERE a.p41Code LIKE @code"), new { code = strCode });
            }
        }
        public BO.p41Project LoadByExternalPID(string externalpid)
        {
            return _db.Load<BO.p41Project>(GetSQL1(" WHERE a.p41ExternalPID=@externalpid"), new { externalpid = externalpid });
        }

        public IEnumerable<BO.p41Project> GetList(BO.myQueryP41 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p41Project>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.p41Project rec, List<BO.FreeFieldInput> lisFFI, List<BO.x69EntityRole_Assign> lisX69)
        {
            if (rec.p42ID == 0)
            {
                return this.ZeroMessage("Chybí typ projektu.");
            }
            var recP42 = _mother.p42ProjectTypeBL.Load(rec.p42ID);

            if (!ValidateBeforeSave(rec, recP42))
            {
                return 0;
            }

            using (var sc = new System.Transactions.TransactionScope())     //ukládání podléhá transakci
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddInt("p41ParentID", rec.p41ParentID, true);
                p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
                p.AddInt("p28ID_Client", rec.p28ID_Client, true);
                p.AddInt("p28ID_Billing", rec.p28ID_Billing, true);
                p.AddInt("p42ID", rec.p42ID, true);
                p.AddInt("p92ID", rec.p92ID, true);
                p.AddInt("p87ID", rec.p87ID, true);
                p.AddInt("p51ID_Billing", rec.p51ID_Billing, true);
                p.AddInt("p51ID_Internal", rec.p51ID_Internal, true);
                p.AddInt("j18ID", rec.j18ID, true);
                p.AddInt("p61ID", rec.p61ID, true);
                p.AddEnumInt("p41WorksheetOperFlag", rec.p41WorksheetOperFlag);
                p.AddEnumInt("p72ID_NonBillable", rec.p72ID_NonBillable,true);
                p.AddEnumInt("p72ID_NonBillable", rec.p72ID_NonBillable,true);
                p.AddInt("j02ID_ContactPerson_DefaultInWorksheet", rec.j02ID_ContactPerson_DefaultInWorksheet, true);

                p.AddString("p41Name", rec.p41Name);
                p.AddString("p41Code", rec.p41Code);
                p.AddString("p41NameShort", rec.p41NameShort);
                p.AddString("p41ExternalPID", rec.p41ExternalPID);
                p.AddString("p41BillingMemo", rec.p41BillingMemo);

                p.AddString("p41InvoiceDefaultText1", rec.p41InvoiceDefaultText1);
                p.AddString("p41InvoiceDefaultText2", rec.p41InvoiceDefaultText2);
                p.AddInt("p41InvoiceMaturityDays", rec.p41InvoiceMaturityDays);

                p.AddInt("p41LimitWipFlag", rec.p41LimitWipFlag);
                p.AddDouble("p41LimitHours_Notification", rec.p41LimitHours_Notification);
                p.AddDouble("p41LimitFee_Notification", rec.p41LimitFee_Notification);

                p.AddEnumInt("p41BillingFlag", rec.p41BillingFlag);
                p.AddEnumInt("p41ReportingFlag", rec.p41ReportingFlag);

                p.AddDateTime("p41PlanFrom", rec.p41PlanFrom);
                p.AddDateTime("p41PlanUntil", rec.p41PlanUntil);

                p.AddBool("p41IsNoNotify", rec.p41IsNoNotify);

                int intPID = _db.SaveRecord("p41Project", p, rec);
                if (intPID > 0)
                {
                    if (!DL.BAS.SaveFreeFields(_db, intPID, lisFFI))
                    {
                        return 0;
                    }
                    if (lisX69 != null && !DL.BAS.SaveX69(_db, "p41", intPID, lisX69))
                    {
                        this.AddMessageTranslated("Error: DL.BAS.SaveX69");
                        return 0;
                    }
                    var pars = new Dapper.DynamicParameters();
                    {
                        pars.Add("p41id", intPID, System.Data.DbType.Int32);
                        pars.Add("j03id_sys", _mother.CurrentUser.pid, System.Data.DbType.Int32);                        
                    }

                    if (_db.RunSp("p41_aftersave", ref pars, false) == "1")
                    {
                        sc.Complete();                        
                    }
                    else
                    {
                        return 0;
                    }
                }
                return intPID;
            }
            
        }

        public bool ValidateBeforeSave(BO.p41Project rec,BO.p42ProjectType recP42)
        {
            if (rec.pid == 0)
            {
                if (!_mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P41_Creator))
                {
                    //ověřit, jakým způsobem může zakládat nové projekty
                    if (_mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P41_Draft_Creator))
                    {
                        rec.p41IsDraft = true;
                    }
                    else
                    {
                        //oprávnění zakládat pod-projekty?
                    }
                }
            }
            
            if (string.IsNullOrEmpty(rec.p41Name))
            {
                return this.FalsehMessage("Chybí vyplnit [Název].");
            }
            if (rec.p41PlanFrom != null && rec.p41PlanUntil==null)
            {
                return this.FalsehMessage("Pokud je plán zahájení vyplněný, musíte vyplnit i datum plánu dokončení.");
            }
            if (rec.p41PlanFrom != null && rec.p41PlanUntil != null)
            {
                if (rec.p41PlanFrom > rec.p41PlanUntil)
                {
                    return this.FalsehMessage("Datum plánovaného zahájení je větší než datum plánovaného dokončení.");
                }
                if ((Convert.ToDateTime(rec.p41PlanUntil) - Convert.ToDateTime(rec.p41PlanFrom)).TotalDays>1000)
                {
                    return this.FalsehMessage("Doba časového plánu projektu musí být menší než 1000 dní.");
                }
                
            }

            if (rec.ValidUntil < DateTime.Now && rec.pid>0)
            {
                //pokus o přesun projektu do archivu
                switch (recP42.p42ArchiveFlag)
                {
                    case BO.p42ArchiveFlagENUM.NoArchive_Waiting_Approve:
                        if (ExistWaitingWorksheetForInvoicing(rec.pid))
                        {
                            return this.FalsehMessage("Projekt nelze přesunout do achivu, dokud v něm existují nevyfakturované úkony. Tuto ochranu může změnit administrátor v nastavení typu projektu.");
                        }
                        break;
                    case BO.p42ArchiveFlagENUM.NoArchive_Waiting_Invoice:
                        if (ExistWaitingWorksheetForInvoicing(rec.pid))
                        {
                            return this.FalsehMessage("Projekt nelze přesunout do achivu, dokud v něm existují rozpracované úkony. Rozpracované úkony lze přesunout do archivu nebo tuto ochranu může změnit administrátor v nastavení typu projektu.");
                        }
                        break;
                }
            }

            switch (rec.p41BillingFlag)
            {
                case BO.p41BillingFlagEnum.FixedPrice:
                    if (rec.p72ID_BillableHours == BO.p72IdENUM._NotSpecified)
                    {
                        return this.FalsehMessage("V projektu s fakturačním režimem [Pevná cena] musíte vyplnit volbu: [Výchozí status fakturovatelných hodin pro schvalování].");
                    }
                    break;
                case BO.p41BillingFlagEnum.WithoutBilling:
                    rec.p51ID_Billing = 0;
                    rec.p72ID_BillableHours = BO.p72IdENUM._NotSpecified;
                    break;
                default:
                    rec.p72ID_BillableHours = BO.p72IdENUM._NotSpecified;
                    break;
            }

            

            return true;
        }

        public BO.p41ProjectSum LoadSumRow(int pid)
        {
            return _db.Load<BO.p41ProjectSum>("EXEC dbo.p41_inhale_sumrow @j03id_sys,@pid", new { j03id_sys = _mother.CurrentUser.pid, pid = pid });
        }

        public BO.p41RecDisposition InhaleRecDisposition(BO.p41Project rec)
        {
            var c = new BO.p41RecDisposition();

            if (rec.j02ID_Owner == _mother.CurrentUser.j02ID || _mother.CurrentUser.IsAdmin || _mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P41_Owner))
            {
                c.OwnerAccess = true; c.ReadAccess = true;
                return c;
            }
            if (_mother.CurrentUser.TestPermission(BO.x53PermValEnum.GR_P41_Reader))
            {
                c.ReadAccess = true;
                return c;
            }

            return c;
        }

        public bool ExistWaitingWorksheetForInvoicing(int p41id)
        {            
            if (_db.Load<BO.GetInteger>("select top 1 p31ID as Value FROM p31worksheet WHERE p41ID=@pid AND p91ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil", new { pid = p41id }) != null)
            {
                return true;
            }
            return false;
        }
        public bool ExistWaitingWorksheetForApproving(int p41id)
        {
            if (_db.Load<BO.GetInteger>("select top 1 p31ID FROM p31worksheet WHERE p41ID=@pid AND p71ID IS NULL AND p91ID IS NULL AND getdate() BETWEEN p31ValidFrom AND p31ValidUntil", new { pid = p41id }) != null)
            {
                return true;
            }
            return false;
        }
    }
}

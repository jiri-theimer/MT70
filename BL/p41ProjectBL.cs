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
        public BO.p41Project LoadByCode(string strCode, int pid_exclude);
        public BO.p41Project LoadByExternalPID(string externalpid);
        public IEnumerable<BO.p41Project> GetList(BO.myQuery mq);
        public int Save(BO.p41Project rec);


    }
    class p41ProjectBL : BaseBL, Ip41ProjectBL
    {
        public p41ProjectBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.p42ID,a.j02ID_Owner,a.p41Name,a.p41NameShort,a.p41Code,a.p41IsDraft,a.p28ID_Client,a.p28ID_Billing,a.p87ID,a.p51ID_Billing,a.p51ID_Internal,a.p92ID,a.b02ID,a.j18ID,a.p61ID,a.p41InvoiceDefaultText1,a.p41InvoiceDefaultText2,a.p41InvoiceMaturityDays,a.p41WorksheetOperFlag,a.p41PlanFrom,a.p41PlanUntil,a.p41LimitHours_Notification,a.p41LimitFee_Notification");
            sb(",p28client.p28Name as Client,p51billing.p51Name as p51Name_Billing,a.p41LimitWipFlag");
            sb(",a.p41TreeLevel,a.p41TreeIndex,a.p41TreePrev,a.p41TreeNext,a.p41TreePath,a.p41TreeOrdinary");
            sb(",p42.p42Name,p92.p92Name,b02.b02Name,j18.j18Name,a.p41ExternalPID,a.p41ParentID,a.p41BillingMemo");
            sb(",j02owner.j02LastName+' '+j02owner.j02FirstName as Owner,p28client.p87ID as p87ID_Client,p42.b01ID,a.p41IsNoNotify,a.p72ID_NonBillable,a.p72ID_BillableHours,a.j02ID_ContactPerson_DefaultInWorksheet,a.j02ID_ContactPerson_DefaultInInvoice");
            sb(",a.p41BillingFlag,a.p41ReportingFlag");
            
            sb(","+_db.GetSQL1_Ocas("p41"));

            sb(" FROM p41Project a INNER JOIN p42ProjectType p42 ON a.p42ID=p42.p42ID");
            sb(" LEFT OUTER JOIN p28Contact p28client ON a.p28ID_Client=p28client.p28ID");
            sb(" LEFT OUTER JOIN p51PriceList p51billing ON a.p51ID_Billing=p51billing.p51ID");
            sb(" LEFT OUTER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID");
            sb(" LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID");

           
            sb(" LEFT OUTER JOIN p87BillingLanguage p87 ON a.p87ID=p87.p87ID");
            
            sb(" LEFT OUTER JOIN j18Region j18 ON a.j18ID=j18.j18ID");
            sb(" LEFT OUTER JOIN p41Project_FreeField p41free ON a.p41ID=p41free.p41ID");
            sb(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID");

            sb(strAppend);


            return sbret();
        }
        public BO.p41Project Load(int pid)
        {
            return _db.Load<BO.p41Project>(GetSQL1(" WHERE a.p41ID=@pid"), new { pid = pid });
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

        public IEnumerable<BO.p41Project> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p41Project>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.p41Project rec)
        {
            if (!ValidateBeforeSave(rec))
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
                p.AddEnumInt("p72ID_NonBillable", rec.p72ID_NonBillable);
                p.AddEnumInt("p72ID_NonBillable", rec.p72ID_NonBillable);
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

                int intPID = _db.SaveRecord("p41Project", p.getDynamicDapperPars(), rec);
                if (intPID > 0)
                {
                    var pars = new Dapper.DynamicParameters();
                    {

                        pars.Add("p41id", intPID, System.Data.DbType.Int32);
                        pars.Add("j03id_sys", _mother.CurrentUser.pid, System.Data.DbType.Int32);
                        pars.Add("x45ids", null, System.Data.DbType.String, System.Data.ParameterDirection.Output, 50);
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

        public bool ValidateBeforeSave(BO.p41Project rec)
        {

            if (string.IsNullOrEmpty(rec.p41Name) || string.IsNullOrEmpty(rec.p41Code))
            {
                this.AddMessage("[Název] a [Kód] jsou povinná pole."); return false;
            }

            if (LoadByCode(rec.p41Code, rec.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("Kód [{0}] již je obsazen jiným polem."), rec.p41Code));
                return false;
            }

            return true;
        }

    }
}

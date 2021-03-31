using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip28ContactBL
    {
        public BO.p28Contact Load(int pid);
        public IEnumerable<BO.p28Contact> GetList(BO.myQueryP28 mq);
        public IEnumerable<BO.o32Contact_Medium> GetList_o32(int p28id);
        public int Save(BO.p28Contact rec, BO.o38Address recO38First, List<BO.o32Contact_Medium> lisO32,List<BO.FreeFieldInput> lisFFI);


    }
    class p28ContactBL : BaseBL, Ip28ContactBL
    {

        public p28ContactBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.p29ID,a.p92ID,a.j02ID_Owner,a.p87ID,a.p51ID_Billing,a.p51ID_Internal,a.b02ID,a.p63ID,a.p28IsCompany,a.p28IsDraft,a.p28Code,a.p28FirstName,a.p28LastName,a.p28TitleBeforeName,a.p28TitleAfterName,a.p28RegID,a.p28VatID,a.p28Person_BirthRegID,a.p28CompanyName,a.p28CompanyShortName,a.p28InvoiceDefaultText1,a.p28InvoiceDefaultText2,a.p28InvoiceMaturityDays,a.p28LimitHours_Notification,a.p28LimitFee_Notification");
            sb(",a.p28Name,p29.p29Name,p92.p92Name,b02.b02Name,p87.p87Name,a.p28RobotAddress,a.p28SupplierID,a.p28SupplierFlag,a.p28ExternalPID,a.p28Round2Minutes,a.p28ICDPH_SK");
            sb(",a.p28TreeLevel,a.p28TreeIndex,a.p28TreePrev,a.p28TreeNext,a.p28TreePath");
            sb(",p51billing.p51Name as p51Name_Billing,j02owner.j02LastName+' '+j02owner.j02FirstName as Owner,a.p28ParentID,a.p28BillingMemo,a.p28Pohoda_VatCode,a.j02ID_ContactPerson_DefaultInWorksheet,a.j02ID_ContactPerson_DefaultInInvoice,a.j61ID_Invoice");

            sb(","+_db.GetSQL1_Ocas("p28") + " FROM p28Contact a");
            sb(" LEFT OUTER JOIN p29ContactType p29 ON a.p29ID=p29.p29ID");
            sb(" LEFT OUTER JOIN p87BillingLanguage p87 ON a.p87ID=p87.p87ID");
            sb(" LEFT OUTER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID");
            sb(" LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID");
            sb(" LEFT OUTER JOIN p51PriceList p51billing ON a.p51ID_Billing=p51billing.p51ID");
            sb(" LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID");
            //sb(" LEFT OUTER JOIN p28Contact_FreeField p28free ON a.p28ID=p28free.p28ID");
            sb(strAppend);
            return sbret();
        }

        public BO.p28Contact Load(int pid)
        {
            return _db.Load<BO.p28Contact>(GetSQL1(" WHERE a.p28ID=@pid"), new { pid = pid });
        }
        public IEnumerable<BO.p28Contact> GetList(BO.myQueryP28 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p28Contact>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.o32Contact_Medium> GetList_o32(int p28id)
        {            
            return _db.GetList<BO.o32Contact_Medium>("select a.*,o33.o33Name FROM o32Contact_Medium a INNER JOIN o33MediumType o33 ON a.o33ID=o33.o33ID WHERE a.p28ID=@p28id",new { p28id = p28id });
        }

        public int Save(BO.p28Contact rec,BO.o38Address recO38First,List<BO.o32Contact_Medium> lisO32, List<BO.FreeFieldInput> lisFFI)
        {
            if (!ValidateBeforeSave(rec, recO38First))
            {
                return 0;
            }

            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();

                p.AddInt("pid", rec.pid);
                p.AddInt("p29ID", rec.p29ID, true);
                p.AddInt("p92ID", rec.p92ID, true);
                p.AddInt("p87ID", rec.p87ID, true);
                p.AddInt("p51ID_Billing", rec.p51ID_Billing, true);
                p.AddInt("p51ID_Internal", rec.p51ID_Internal, true);
                p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
                p.AddInt("j02ID_ContactPerson_DefaultInWorksheet", rec.j02ID_ContactPerson_DefaultInWorksheet, true);
                p.AddInt("j02ID_ContactPerson_DefaultInInvoice", rec.j02ID_ContactPerson_DefaultInInvoice, true);
                p.AddInt("p63ID", rec.p63ID, true);
                p.AddInt("p28ParentID", rec.p28ParentID, true);
                p.AddInt("j61ID_Invoice", rec.j61ID_Invoice, true);

                p.AddEnumInt("p28SupplierFlag", rec.p28SupplierFlag);
                p.AddInt("j61ID_Invoice", rec.j61ID_Invoice, true);

                p.AddBool("p28IsCompany", rec.p28IsCompany);
                p.AddString("p28CompanyName", rec.p28CompanyName);
                p.AddString("p28CompanyShortName", rec.p28CompanyShortName);                
                p.AddString("p28FirstName", rec.p28FirstName);
                p.AddString("p28LastName", rec.p28LastName);
                p.AddString("p28TitleBeforeName", rec.p28TitleBeforeName);
                p.AddString("p28TitleAfterName", rec.p28TitleAfterName);

                p.AddString("p28RegID", rec.p28RegID);
                p.AddString("p28VatID", rec.p28VatID);
                p.AddString("p28RegID", rec.p28RegID);
                p.AddString("p28ICDPH_SK", rec.p28ICDPH_SK);
                p.AddString("p28Person_BirthRegID", rec.p28Person_BirthRegID);
                p.AddString("p28ExternalPID", rec.p28ExternalPID);                
                p.AddString("p28SupplierID", rec.p28SupplierID);

                p.AddString("p28BillingMemo", rec.p28BillingMemo);
                p.AddString("p28InvoiceDefaultText1", rec.p28InvoiceDefaultText1);
                p.AddString("p28InvoiceDefaultText2", rec.p28InvoiceDefaultText2);
                p.AddString("p28Pohoda_VatCode", rec.p28Pohoda_VatCode);
                p.AddString("p28Pohoda_VatCode", rec.p28Pohoda_VatCode);
                p.AddString("p28Pohoda_VatCode", rec.p28Pohoda_VatCode);
                p.AddString("p28Pohoda_VatCode", rec.p28Pohoda_VatCode);

                p.AddInt("p28InvoiceMaturityDays", rec.p28InvoiceMaturityDays);
                p.AddInt("p28Round2Minutes", rec.p28Round2Minutes);
                p.AddDouble("p28LimitHours_Notification", rec.p28LimitHours_Notification);
                p.AddDouble("p28LimitFee_Notification", rec.p28LimitFee_Notification);


                int intPID = _db.SaveRecord("p28Contact", p, rec);

                if (intPID > 0)
                {
                    DL.BAS.SaveFreeFields(_db, intPID, lisFFI);

                    if (recO38First != null)
                    {
                       int intO38ID = _mother.o38AddressBL.Save(recO38First,intPID,1);
                       
                    }
                }

                sc.Complete();

                return intPID;
            }
                
        }


        private bool ValidateBeforeSave(BO.p28Contact rec, BO.o38Address recO38First)
        {

            if (string.IsNullOrEmpty(rec.p28CompanyName) && string.IsNullOrEmpty(rec.p28LastName))
            {
                this.AddMessage("Chybí název nebo příjmení."); return false;
            }
            if (recO38First != null)
            {
                if (!_mother.o38AddressBL.ValidateBeforeSave(recO38First))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

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
        public IEnumerable<BO.p28Contact> GetList(BO.myQuery mq);
        public int Save(BO.p28Contact rec);


    }
    class p28ContactBL : BaseBL, Ip28ContactBL
    {

        public p28ContactBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,b01.b01Name,x38.x38Name," + _db.GetSQL1_Ocas("p28") + " FROM p28Contact a");
            sb(" LEFT OUTER JOIN b01WorkflowTemplate b01 ON a.b01ID=b01.b01ID");
            sb(" LEFT OUTER JOIN x38CodeLogic x38 ON a.x38ID=x38.x38ID");
            sb(strAppend);
            return sbret();
        }

        public BO.p28Contact Load(int pid)
        {
            return _db.Load<BO.p28Contact>(GetSQL1(" WHERE a.p28ID=@pid"), new { pid = pid });
        }
        public IEnumerable<BO.p28Contact> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p28Contact>(fq.FinalSql, fq.Parameters);
        }


        public int Save(BO.p28Contact rec,List<BO.o37Contact_Address> lisO37,List<BO.o32Contact_Medium> lisO32,List<BO.p30Contact_Person> lisP30)
        {
            if (!ValidateBeforeSave(rec))
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
                    if (lisO37 != null)
                    {
                        if (rec.pid > 0)
                        {
                            _db.RunSql("DELETE FROM o37Contact_Address WHERE p28ID=@pid", new { pid = intPID });
                        }
                        foreach(var c in lisO37)
                        {
                            var recO38 = new BO.o38Address();
                            _db.RunSql("INSERT INTO o37Contact_Address(p28ID,o38ID,o36ID) VALUES(@p28id,@o38id,@o36id)", new { p28id = intPID });
                        }
                    }
                }

                return intPID;
            }
                
        }


        private bool ValidateBeforeSave(BO.p28Contact rec)
        {

            if (string.IsNullOrEmpty(rec.p28Name))
            {
                this.AddMessage("Chybí název."); return false;
            }


            return true;
        }
    }
}

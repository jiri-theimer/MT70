using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip86BankAccountBL
    {
        public BO.p86BankAcc Load(int pid);
        public BO.p86BankAcc LoadInvoiceAccount(int p91id);
        public IEnumerable<BO.p86BankAcc> GetList(BO.myQuery mq);
        public int Save(BO.p86BankAcc rec);

    }
    class p86BankAccountBL : BaseBL, Ip86BankAccountBL
    {
        public p86BankAccountBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("p86"));
            sb(" FROM p86BankAccount a");
            sb(strAppend);
            return sbret();
        }
        public BO.p86BankAcc Load(int pid)
        {
            return _db.Load<BO.p86BankAcc>(GetSQL1(" WHERE a.p86ID=@pid"), new { pid = pid });
        }

        public BO.p86BankAcc LoadInvoiceAccount(int p91id)   //vrátí bankovní účet pro fakturu p91id
        {
            return _db.Load<BO.p86BankAcc>(GetSQL1(" WHERE a.p86ID=dbo.p91_get_p86id(@p91id)"), new { p91id = p91id });
        }

        public IEnumerable<BO.p86BankAcc> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p86BankAcc>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.p86BankAcc rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("p86Name", rec.p86Name);
            p.AddString("p86BankName", rec.p86BankName);
            p.AddString("p86BankAccount", rec.p86BankAccount);
            p.AddString("p86BankCode", rec.p86BankCode);
            p.AddString("p86SWIFT", rec.p86SWIFT);
            p.AddString("p86IBAN", rec.p86IBAN);
            p.AddString("p86BankAddress", rec.p86BankAddress);

            return _db.SaveRecord("p86BankAccount", p, rec);


        }
        private bool ValidateBeforeSave(BO.p86BankAcc rec)
        {
            if (string.IsNullOrEmpty(rec.p86Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (string.IsNullOrEmpty(rec.p86BankAccount))
            {
                this.AddMessage("Chybí vyplnit [Číslo účtu]."); return false;
            }
            var lis = GetList(new BO.myQuery("p86") { IsRecordValid = null });
            if (lis.Where(p=>p.p86BankAccount.Replace(" ","")==rec.p86BankAccount.Replace(" ","") && p.p86BankCode==rec.p86BankCode && p.pid !=rec.pid).Any())
            {                
                this.AddMessage(string.Format("Číslo účtu {0}/{1} je již vyplněné v jiném bankovním účtu.", rec.p86BankAccount, rec.p86BankCode));
                return false;
            }
            
            if (!string.IsNullOrEmpty(rec.p86IBAN) && lis.Where(p => p.p86IBAN != null && p.p86IBAN.Replace(" ", "") == rec.p86IBAN.Replace(" ", "") && p.pid != rec.pid).Any())
            {
                this.AddMessage(string.Format("IBAN [{0}] je již vyplněný v jiném bankovním účtu.", rec.p86IBAN));
                return false;
            }


            return true;
        }

    }
}
